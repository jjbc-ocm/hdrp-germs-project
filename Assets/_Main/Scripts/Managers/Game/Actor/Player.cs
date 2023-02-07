/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace TanksMP
{
    public class Player : ActorManager, IPunObservable
    {
        public static Player Mine;

        #region Network Sync

        private Vector3 shipRotation;

        #endregion

        #region Serializable

        [SerializeField]
        private GPShipDesc data;

        [Header("Other Properties")]

        [SerializeField]
        private SkillData attack;

        [SerializeField]
        private SkillData skill;

        #endregion

        private float nextAttackTime;

        private float nextSkillTime;

        private Vector2 moveDir;

        private Vector2 prevMoveDir;

        private bool isRespawning;

        #region Accessors

        public GPShipDesc Data { get => data; }

        public SkillData Attack { get => attack; }

        public SkillData Skill { get => skill; }

        public float NextAttackTime { get => nextAttackTime; }

        public float NextSkillTime { get => nextSkillTime; }

        public bool IsRespawning { get => isRespawning; }

        #endregion

        #region Override

        protected override void OnTriggerEnterCalled(Collider col)
        {
            if (!photonView.IsMine) return;

            var collectibleZone = col.GetComponent<CollectibleZone>();

            var collectibleTeam = col.GetComponent<CollectibleTeam>();

            var collectible = col.GetComponent<Collectible>();

            /* Handle collision to the collectible zone */
            if (collectibleZone != null &&
                GetTeam() == collectibleZone.teamIndex &&
                HasChest())
            {
                HasChest(false);

                collectibleZone.OnDropChest();

                GPSManager.Instance.ClearDestination();
            }

            /* Handle collision with chests */
            else if (collectibleTeam != null)
            {
                collectibleTeam.Obtain(this);

                var destination = GetTeam() == 0
                    ? GameManager.Instance.zoneRed.transform.position
                    : GameManager.Instance.zoneBlue.transform.position;

                GPSManager.Instance.SetDestination(destination);
            }

            /* Handle collision to normal item */
            else if (collectible != null)
            {
                collectible.Obtain(this);
            }
        }

        protected override void OnTriggerExitCalled(Collider col)
        {

        }

        #endregion

        #region Unity

        private void Start()
        {
            gameObject.layer = GetTeam() == PhotonNetwork.LocalPlayer.GetTeam() ?
                LayerMask.NameToLayer(SOManager.Instance.Constants.LayerAlly) :
                LayerMask.NameToLayer(SOManager.Instance.Constants.LayerEnemy);

            Stat.Initialize();

            Aim.Initialize(
                () =>
                {
                    if (!ShopManager.Instance.UI.gameObject.activeSelf)
                        ExecuteActionAim(attack, true);
                },
                () =>
                {
                    if (!ShopManager.Instance.UI.gameObject.activeSelf)
                        ExecuteActionAim(skill, false);
                },
                (aimPosition, autoTarget) =>
                {
                    if (!ShopManager.Instance.UI.gameObject.activeSelf)
                        ExecuteActionInstantly(aimPosition, autoTarget, false);
                },
                () =>
                {
                    return Stat.Mana >= attack.MpCost && Time.time > nextAttackTime;
                },
                () =>
                {
                    return Stat.Mana >= skill.MpCost && Time.time > nextSkillTime;
                });

            if (!photonView.IsMine || IsBot) return;

            Mine = this;

            Globals.ROOM_NAME = PhotonNetwork.CurrentRoom.Name;

            GameCameraManager.Instance.SetTarget(transform);
        }

        private void Update()
        {
            if (!photonView.IsMine && !IsBot) return;

            if (Input.IsShop && !ChatManager.Instance.UI.IsMaximized)
            {
                ShopManager.Instance.ToggleShop();
            }

            if (Input.IsScoreBoard)
            {
                if (!ScoreBoardUI.Instance.gameObject.activeSelf)
                {
                    ScoreBoardUI.Instance.Open((self) =>
                    {
                        self.Data = new List<List<Player>>
                    {
                        GameManager.Instance.Team1Ships,
                        GameManager.Instance.Team2Ships
                    };
                    });
                }
                else
                {
                    ScoreBoardUI.Instance.Close();
                }
            }
        }

        private void FixedUpdate()
        {
            if (!photonView.IsMine || isRespawning) return;

            /* Update movement */
            if (!ShopManager.Instance.UI.gameObject.activeSelf)
            {
                if (Input.Move.x == 0 && Input.Move.y == 0)
                {
                    moveDir.x += (0 - prevMoveDir.x) * 0.1f;
                    moveDir.y += (0 - prevMoveDir.y) * 0.1f;
                }
                else
                {
                    moveDir.x += (Input.Move.x - prevMoveDir.x) * 0.1f;
                    moveDir.y += (Input.Move.y - prevMoveDir.y) * 0.1f;

                    var acceleration = Input.IsSprint ? 2 : 1;

                    var moveForce = transform.forward * moveDir.y * Stat.MoveSpeed * acceleration;

                    var moveTorque = transform.up * moveDir.x * Stat.MoveSpeed;

                    RigidBody.AddForce(moveForce);

                    RigidBody.AddTorque(moveTorque);
                }
            } 

            /* Update rotation */
            shipRotation = new Vector3(
                moveDir.y * -10,
                SoundVisuals.RendererAnchor.transform.localEulerAngles.y,
                moveDir.x * -10);

            SoundVisuals.RendererAnchor.transform.localRotation = Quaternion.Euler(shipRotation);

            /* Cache it because, we need to accumulate the movement force */
            prevMoveDir = moveDir;
        }

        #endregion

        #region Photon

        void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(shipRotation);
            }
            else
            {
                shipRotation = (Vector3)stream.ReceiveNext();
            }
        }

        [PunRPC]
        public void RpcSendChat(string sender, string message, int team, long time)
        {
            ChatManager.Instance.SendChat(sender, message, team, time);
        }

        [PunRPC]
        public void RpcAction(Vector3 fromPosition, Vector3 targetPosition, int targetActorId, bool isAttack)
        {
            var action = isAttack ? attack : skill;

            /* Steps
             * 1. Calculate the rotation ased on position and target
             * 2. Spawn action.Effect based on position, and rotation
             * 3. Any trail reset or sound effects should be done on the actual object spawned
             */
            //var vPosition = new Vector3(position[0], position[1], position[2]);

            //var vTarget = new Vector3(target[0], target[1], target[2]);

            var forward = fromPosition - targetPosition;//vTarget - vPosition;

            var rotation = Quaternion.LookRotation(forward);

            var effect = action.IsSpawnOnAim
                ? Instantiate(action.Effect, targetPosition, rotation)
                : Instantiate(action.Effect, fromPosition, rotation);

            //effect.Initialize(this, PhotonView.Find(autoTargetPhotonID)?.GetComponent<ActorManager>() ?? null); // TODO: 3

            var targetActor = PhotonView.Find(targetActorId)?.GetComponent<ActorManager>() ?? null;

            effect.Initialize(this, targetPosition, targetActor);
        }

        [PunRPC]
        public void RpcDestroy(int attackerId)
        {
            ToggleFunction(false);

            var attackerView = PhotonView.Find(attackerId);

            if (PhotonNetwork.IsMasterClient)
            {
                //send player back to the team area, this will get overwritten by the exact position from the client itself later on
                //we just do this to avoid players "popping up" from the position they died and then teleporting to the team area instantly
                //this is manipulating the internal PhotonTransformView cache to update the networkPosition variable
                /*GetComponent<PhotonTransformView>().OnPhotonSerializeView(
                    new PhotonStream(
                        false,
                        new object[]
                        {
                            //GameManager.Instance.GetSpawnPosition(photonView.GetTeam()),
                            GameNetworkManager.Instance.SpawnPoints[photonView.GetTeam()].position,
                            Vector3.zero,
                            Quaternion.identity
                        }),
                    new PhotonMessageInfo());*/

                ResetPosition(false);
            }

            if (photonView.IsMine)
            {
                if (!IsBot)
                {
                    GameCameraManager.Instance.SetTarget(attackerView.transform);
                }
                
                photonView.RPC("RpcBroadcastKillStatement", RpcTarget.All, attackerView.ViewID, photonView.ViewID);

                StartCoroutine(SpawnRoutine());
            }

            if (onDieEvent != null)
            {
                onDieEvent.Invoke(photonView.ViewID);
            }
        }

        [PunRPC]
        public void RpcBroadcastKillStatement(int attackerId, int defenderId)
        {
            var winner = PhotonView.Find(attackerId).GetComponent<ActorManager>();

            var loser = PhotonView.Find(defenderId).GetComponent<ActorManager>();

            GameManager.Instance.ui.OpenKillStatement(winner, loser);
        }

        [PunRPC]
        public void RpcRevive()
        {
            ToggleFunction(true);

            if (photonView.IsMine)
            {
                ResetPosition(true);
            }
        }

        [PunRPC]
        public void RpcGameOver(int winnerTeamIndex)
        {
            GameManager.Instance.DisplayGameOver(winnerTeamIndex);
        }

        [PunRPC]
        public override void RpcDamageHealth(int amount, int attackerId)
        {
            /* Do not damage this ship if respawning */
            if (isRespawning) return;

            Stat.AddHealth(-amount);

            GPNumberSpawnerSystem.m_instance.SpawnDamageNumber(amount, transform.position);

            if (Stat.Health <= 0)
            {
                /* Add score to opponent */
                var attacker = PhotonView.Find(attackerId).GetComponent<ActorManager>();

                if (attacker != null)
                {
                    var attackerTeam = attacker.GetTeam();

                    if (GetTeam() != attackerTeam)
                    {
                        GameManager.Instance.AddScore(ScoreType.Kill, attackerTeam);

                        GPRewardSystem.m_instance.AddGoldToPlayer(attacker.photonView.Owner, "Kill");
                    }

                    /* If the attacker is me, add kill count, then broadcast it */
                    if (attacker.photonView.IsMine && attacker.TryGetComponent(out Player attackerPlayer))
                    {
                        attackerPlayer.Stat.AddKill();

                        photonView.RPC("RpcBroadcastKillStatement", RpcTarget.All, attackerId, photonView.ViewID);
                    }
                }

                /* Handle collected chest */
                if (HasChest())
                {
                    HasChest(false);

                    var chest = ItemSpawnerManager.Instance.Spawners.FirstOrDefault(i => i.IsChest).Prefab;

                    PhotonNetwork.InstantiateRoomObject(chest.name, transform.position, Quaternion.identity);

                    GPSManager.Instance.ClearDestination();
                }
                
                /* Reset stats */
                Stat.SetHealth(Stat.MaxHealth);

                Stat.SetMana(Stat.MaxMana);

                Stat.AddDeath();

                /* Broadcast to all player that this ship is destroyed */
                photonView.RPC("RpcDestroy", RpcTarget.All, attackerId);

                GuideManager.Instance.TryAddShopGuide();
            }
        }

        #endregion

        #region Public

        public void ResetPosition(bool isCameraFollow)
        {
            if (isCameraFollow && !IsBot)
            {
                GameCameraManager.Instance.SetTarget(transform);
            }
            
            transform.position = GameNetworkManager.Instance.SpawnPoints[GetTeam()].position;
            transform.rotation = GameNetworkManager.Instance.SpawnPoints[GetTeam()].rotation;

            RigidBody.velocity = Vector3.zero;
            RigidBody.angularVelocity = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }

        #endregion

        #region Private

        private void ExecuteActionAim(SkillData action, bool isAttack)
        {
            var instantAim =
                action.Aim == AimType.None ? transform.position :
                action.Aim == AimType.WhileExecute ? transform.position + transform.forward * 999f :
                Vector3.zero;

            if (action.Aim == AimType.None || action.Aim == AimType.WhileExecute)
            {
                ExecuteActionInstantly(instantAim, null, isAttack);
            }
        }

        private void ExecuteActionInstantly(Vector3 aimPosition, ActorManager autoTarget, bool isAttack)
        {
            var action = isAttack ? attack : skill;

            if (isAttack)
            {
                nextAttackTime = Time.time + Constants.MOVE_SPEED_TO_SECONDS_RATIO / Stat.AttackSpeed;
            }
            else
            {
                nextSkillTime = Time.time + action.Cooldown * (1 - Inventory.StatModifier.BuffCooldown - Status.StatModifier.BuffCooldown);
            }

            Stat.AddMana(-action.MpCost);

            var offset = Vector3.up * 2;

            photonView.RPC(
                "RpcAction",
                RpcTarget.AllViaServer,
                transform.position + offset,
                aimPosition + offset,
                autoTarget?.photonView.ViewID ?? -1,
                isAttack);
        }

        private IEnumerator SpawnRoutine()
        {
            isRespawning = true;

            float targetTime = Time.time + Constants.RESPAWN_TIME;

            while (targetTime - Time.time > 0)
            {
                GameManager.Instance.ui.SetSpawnDelay(targetTime - Time.time);

                yield return null;
            }

            GameManager.Instance.ui.DisableDeath();

            isRespawning = false;

            photonView.RPC("RpcRevive", RpcTarget.All);
        }

        private void ToggleFunction(bool toggle)
        {
            SoundVisuals.RendererAnchor.SetActive(toggle);

            foreach (var collider in Colliders)
            {
                collider.enabled = toggle;
            }
        }

        #endregion
    }
}