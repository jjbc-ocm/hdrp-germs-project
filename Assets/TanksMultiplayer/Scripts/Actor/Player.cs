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

namespace TanksMP
{
    public class Player : ActorManager, IPunObservable
    {
        public static Player Mine;

        #region Network Sync

        private Vector3 shipRotation;

        #endregion

        [Header("Other Properties")]

        [SerializeField]
        private SkillData attack;

        [SerializeField]
        private SkillData skill;

        [SerializeField]
        private Collider[] colliders;

        private FollowTarget camFollow;

        private Rigidbody rigidBody;

        private AimManager aim;

        private PlayerSoundVisualManager soundVisuals;

        private PlayerStatManager stat;

        private PlayerStatusManager status;

        private PlayerInventoryManager inventory;

        private float nextAttackTime;

        private float nextSkillTime;

        private Vector2 moveDir;

        private Vector2 prevMoveDir;

        private bool isRespawning;

        

        

        





        public SkillData Attack { get => attack; }

        public SkillData Skill { get => skill; }

        public FollowTarget CamFollow { get => camFollow; }

        public PlayerSoundVisualManager SoundVisuals
        {
            get
            {
                if (soundVisuals == null)
                {
                    soundVisuals = GetComponent<PlayerSoundVisualManager>();
                }

                return soundVisuals;
            }
        }

        public PlayerStatManager Stat 
        {
            get
            {
                if (stat == null)
                {
                    stat = GetComponent<PlayerStatManager>();
                }

                return stat;
            }
        }

        public PlayerStatusManager Status
        {
            get
            {
                if (status == null)
                {
                    status = GetComponent<PlayerStatusManager>();
                }

                return status;
            }
        }

        public PlayerInventoryManager Inventory
        {
            get
            {
                if (inventory == null)
                {
                    inventory = GetComponent<PlayerInventoryManager>();
                }

                return inventory;
            }
        }

        public float NextAttackTime { get => nextAttackTime; }

        public float NextSkillTime { get => nextSkillTime; }

        public bool IsRespawning { get => isRespawning; }
        


        #region Unity

        void Start()
        {
            if (!photonView.IsMine) return;

            Mine = this;

            aim = GetComponent<AimManager>();

            rigidBody = GetComponent<Rigidbody>();

            camFollow = FindObjectOfType<FollowTarget>();

            stat = GetComponent<PlayerStatManager>();

            status = GetComponent<PlayerStatusManager>();

            soundVisuals = GetComponent<PlayerSoundVisualManager>();

            aim.Initialize(
                () =>
                {
                    ExecuteActionAim(attack, true);
                },
                () =>
                {
                    ExecuteActionAim(skill, false);
                },
                (aimPosition, autoTarget) =>
                {
                    ExecuteActionInstantly(aimPosition, autoTarget, false);
                },
                () =>
                {
                    return stat.Mana >= attack.MpCost && Time.time > nextAttackTime;
                },
                () =>
                {
                    return stat.Mana >= skill.MpCost && Time.time > nextSkillTime;
                });

            camFollow.target = transform;
        }

        void Update()
        {
            if (!photonView.IsMine) return;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                ShopManager.Instance.ToggleShop();
            }
        }

        void FixedUpdate()
        {
            if (!photonView.IsMine && !isRespawning) return;

            /* Update movement */
            if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            {
                moveDir.x += (0 - prevMoveDir.x) * 0.1f;
                moveDir.y += (0 - prevMoveDir.y) * 0.1f;
            }
            else
            {
                moveDir.x += (Input.GetAxis("Horizontal") - prevMoveDir.x) * 0.1f;
                moveDir.y += (Input.GetAxis("Vertical") - prevMoveDir.y) * 0.1f;
                ExecuteMove(moveDir);
            }

            /* Update rotation */
            shipRotation = new Vector3(
                moveDir.y * -10,
                soundVisuals.RendererAnchor.transform.localEulerAngles.y,
                moveDir.x * -10);

            soundVisuals.RendererAnchor.transform.localRotation = Quaternion.Euler(shipRotation);

            /* Cache it because, we need to accumulate the movement force */
            prevMoveDir = moveDir;
        }

        void OnTriggerEnter(Collider col)
        {
            if (!photonView.IsMine) return;

            var collectibleZone = col.GetComponent<CollectibleZone>();

            var collectible = col.GetComponent<Collectible>();

            /* Handle collision to the collectible zone */
            if (collectibleZone != null && 
                photonView.GetTeam() == collectibleZone.teamIndex && 
                photonView.HasChest())
            {
                photonView.HasChest(false);

                collectibleZone.OnDropChest();

                GPSManager.Instance.ClearDestination();
            }

            /* Handle collision to normal item */
            else if (collectible != null)
            {
                collectible.Obtain(this);

                var destination = photonView.GetTeam() == 0
                    ? GameManager.GetInstance().zoneRed.transform.position
                    : GameManager.GetInstance().zoneBlue.transform.position;

                GPSManager.Instance.SetDestination(destination);
            }
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
        public void RpcAction(float[] position, float[] target, int autoTargetPhotonID, bool isAttack)
        {
            var action = isAttack ? attack : skill;

            /* Steps
             * 1. Calculate the rotation ased on position and target
             * 2. Spawn action.Effect based on position, and rotation
             * 3. Any trail reset or sound effects should be done on the actual object spawned
             */
            var vPosition = new Vector3(position[0], position[1], position[2]);

            var vTarget = new Vector3(target[0], target[1], target[2]);

            var forward = vTarget - vPosition;

            var rotation = Quaternion.LookRotation(forward);

            var effect = action.IsSpawnOnAim
                ? Instantiate(action.Effect, vTarget, rotation)
                : Instantiate(action.Effect, vPosition, rotation);

            effect.Initialize(this, PhotonView.Find(autoTargetPhotonID)?.GetComponent<Player>() ?? null); // TODO: 3
        }

        [PunRPC]
        public void RpcDestroy(int attackerId)
        {
            ToggleFunction(false);

            var attackerView = attackerId > 0 ? PhotonView.Find(attackerId) : null;

            /*if (explosionFX)
            {
                PoolManager.Spawn(explosionFX, transform.position, transform.rotation);
            }

            if (explosionClip)
            {
                AudioManager.Play3D(explosionClip, transform.position);
            }*/

            if (PhotonNetwork.IsMasterClient)
            {
                //send player back to the team area, this will get overwritten by the exact position from the client itself later on
                //we just do this to avoid players "popping up" from the position they died and then teleporting to the team area instantly
                //this is manipulating the internal PhotonTransformView cache to update the networkPosition variable
                GetComponent<PhotonTransformView>().OnPhotonSerializeView(
                    new PhotonStream(
                        false,
                        new object[]
                        {
                            GameManager.GetInstance().GetSpawnPosition(photonView.GetTeam()),
                            Vector3.zero,
                            Quaternion.identity
                        }),
                    new PhotonMessageInfo());
            }

            if (photonView.IsMine)
            {
                if (attackerView != null)
                {
                    camFollow.target = attackerView.transform;

                    camFollow.HideMask(true);

                    GameManager.GetInstance().ui.SetDeathText(
                        attackerView.GetName(),
                        GameManager.GetInstance().teams[attackerView.GetTeam()]);
                }

                StartCoroutine(SpawnRoutine());
            }

            if (onDieEvent != null)
            {
                onDieEvent.Invoke(photonView.ViewID);
            }
        }

        [PunRPC]
        public void RpcRevive()
        {
            ToggleFunction(true);

            if (photonView.IsMine)
            {
                ResetPosition();
            }
        }

        [PunRPC]
        public void RpcGameOver(byte teamIndex)
        {
            GameManager.GetInstance().DisplayGameOver(teamIndex);
        }

        [PunRPC]
        public override void RpcDamageHealth(int amount, int attackerId)
        {
            stat.AddHealth(-amount);

            if (stat.Health <= 0)
            {
                /* Add score to opponent */
                var attacker = PhotonView.Find(attackerId);

                if (attacker != null)
                {
                    var attackerTeam = attacker.GetTeam();

                    if (photonView.GetTeam() != attackerTeam)
                    {
                        GameManager.GetInstance().AddScore(ScoreType.Kill, attackerTeam);

                        GPRewardSystem.m_instance.AddGoldToPlayer(attacker.Owner, "Kill");
                    }
                }

                /* Handle collected chest */
                photonView.HasChest(false);

                var chest = ItemSpawnerManager.Instance.Spawners.FirstOrDefault(i => i.IsChest).Prefab;

                PhotonNetwork.InstantiateRoomObject(chest.name, transform.position, Quaternion.identity);

                GPSManager.Instance.ClearDestination();

                /* Reset stats */
                stat.AddHealth(stat.MaxHealth);

                stat.AddMana(stat.MaxMana);

                photonView.RPC("RpcDestroy", RpcTarget.All, attackerId);
            }
        }

        #endregion

        #region Public

        public void ResetPosition()
        {
            camFollow.target = transform;
            camFollow.HideMask(false);

            transform.position = GameManager.GetInstance().GetSpawnPosition(photonView.GetTeam());

            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }

        #endregion

        #region Private

        private void ExecuteMove(Vector2 direction)
        {
            //if direction is not zero, rotate player in the moving direction relative to camera
            if (direction != Vector2.zero)
            {
                float x = direction.x * Time.deltaTime * 1.5f * status.BuffMoveSpeed;

                float z = 1;

                var forward = new Vector3(x, 0, z);

                transform.rotation = Quaternion.LookRotation(forward) * Quaternion.Euler(0, camFollow.CamTransform.eulerAngles.y, 0);
            }

            var acceleration = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? 2 : 1;

            Vector3 moveForce = transform.forward * direction.y * stat.MoveSpeed * acceleration;

            rigidBody.AddForce(moveForce);
        }

        private void ExecuteActionAim(SkillData action, bool isAttack)
        {
            var instantAim =
                action.Aim == AimType.None ? transform.position :
                action.Aim == AimType.WhileExecute ? transform.position + transform.forward :
                Vector3.zero;

            if (action.Aim == AimType.None || action.Aim == AimType.WhileExecute)
            {
                ExecuteActionInstantly(instantAim, null, isAttack);
            }
        }

        private void ExecuteActionInstantly(Vector3 aimPosition, Player autoTarget, bool isAttack)
        {
            var action = isAttack ? attack : skill;

            if (isAttack)
            {
                nextAttackTime = Time.time + Constants.MOVE_SPEED_TO_SECONDS_RATIO / stat.AttackSpeed;
            }
            else
            {
                nextSkillTime = Time.time + action.Cooldown * (1 - inventory.StatModifier.BuffCooldown);
            }

            stat.AddMana(-action.MpCost);

            var offset = 2;

            photonView.RPC(
                "RpcAction",
                RpcTarget.AllViaServer,
                new float[] { transform.position.x, transform.position.y + offset, transform.position.z },
                new float[] { aimPosition.x, aimPosition.y + offset, aimPosition.z },
                autoTarget?.photonView.ViewID ?? -1,
                isAttack);
        }

        private IEnumerator SpawnRoutine()
        {
            isRespawning = true;

            float targetTime = Time.time + 10;

            while (targetTime - Time.time > 0)
            {
                GameManager.GetInstance().ui.SetSpawnDelay(targetTime - Time.time);

                yield return null;
            }

            GameManager.GetInstance().ui.DisableDeath();

            isRespawning = false;

            photonView.RPC("RpcRevive", RpcTarget.All);
        }

        private void ToggleFunction(bool toggle)
        {
            soundVisuals.RendererAnchor.SetActive(toggle);

            foreach (var collider in colliders)
            {
                collider.enabled = toggle;
            }
        }

        

        

        #endregion
    }
}