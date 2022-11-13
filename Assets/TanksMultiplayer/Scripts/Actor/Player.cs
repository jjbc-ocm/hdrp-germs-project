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
        [Header("Stats")]

        [SerializeField]
        private int maxHealth = 175;

        [SerializeField]
        private int maxMana = 100;

        [SerializeField]
        private int attackDamage = 50;

        [SerializeField]
        private int abilityPower = 50;

        [SerializeField]
        private int armor = 50;

        [SerializeField]
        private int resist = 50;

        [SerializeField]
        private int attackSpeed = 50;

        [SerializeField]
        private int moveSpeed = 50;

        [Header("Visual and Sound Effects")]

        [SerializeField]
        private Sprite spriteIcon;

        [SerializeField]
        private AudioClip shotClip;

        [SerializeField]
        private AudioClip explosionClip;

        [SerializeField]
        private GameObject shotFX;

        [SerializeField]
        private GameObject explosionFX;

        [Header("Other Properties")]

        [SerializeField]
        private SkillData attack;

        [SerializeField]
        private SkillData skill;

        [SerializeField]
        private Collider[] colliders;

        [SerializeField]
        private GameObject rendererAnchor;

        [SerializeField]
        private GameObject iconIndicator;

        private FollowTarget camFollow;

        private Rigidbody rigidBody;

        private AimManager aim;

        private float nextFire;

        private Vector2 moveDir;

        private Vector2 prevMoveDir;

        

        #region Network Sync

        private int health;

        private int mana;

        private Vector3 shipRotation;

        #endregion

        public int MaxHealth { get => maxHealth; }

        public int MaxMana { get => maxMana; }

        public Sprite SpriteIcon { get => spriteIcon; }

        public SkillData Skill { get => skill; }

        public GameObject IconIndicator { get => iconIndicator; }

        public FollowTarget CamFollow { get => camFollow; }

        public int Health { get => health; }

        public int Mana { get => mana; }

        public void AddHealth(int amount)
        {
            health = Mathf.Clamp(health + amount, 0, maxHealth);
        }

        public void AddMana(int amount)
        {
            mana = Mathf.Clamp(mana + amount, 0, maxMana);
        }


        #region Unity

        void Start()
        {
            if (!photonView.IsMine) return;

            health = maxHealth;

            mana = maxMana;

            StartCoroutine(YieldManaAutoRegen(1));

            if (GameManager.GetInstance() != null)
            {
                GameManager.GetInstance().localPlayer = this;
            }

            aim = GetComponent<AimManager>();

            rigidBody = GetComponent<Rigidbody>();

            camFollow = FindObjectOfType<FollowTarget>();

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
                });

            camFollow.target = transform;
        }

        void FixedUpdate()
        {
            if (!photonView.IsMine) return;

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
                rendererAnchor.transform.localEulerAngles.y,
                moveDir.x * -10);

            rendererAnchor.transform.localRotation = Quaternion.Euler(shipRotation);

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
            }

            /* Handle collision to normal item */
            else if (collectible != null)
            {
                collectible.Obtain(this);
            }
        }

        #endregion

        #region Photon

        void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(health);
                stream.SendNext(mana);
                stream.SendNext(shipRotation);
            }
            else
            {
                health = (int)stream.ReceiveNext();
                mana = (int)stream.ReceiveNext();
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

            if (explosionFX)
            {
                PoolManager.Spawn(explosionFX, transform.position, transform.rotation);
            }

            if (explosionClip)
            {
                AudioManager.Play3D(explosionClip, transform.position);
            }

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
            AddHealth(-amount);

            if (health <= 0)
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

                /* Reset stats */
                health = maxHealth;

                mana = MaxMana;

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
                float x = direction.x * Time.deltaTime * 1.5f;// * (1 + direction.y * -0.5f) * Time.deltaTime;

                float z = 1;

                var forward = new Vector3(x, 0, z);

                transform.rotation = Quaternion.LookRotation(forward) * Quaternion.Euler(0, camFollow.CamTransform.eulerAngles.y, 0);
            }

            var acceleration = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? 2 : 1;

            Vector3 moveForce = transform.forward * direction.y * moveSpeed * acceleration;

            rigidBody.AddForce(moveForce);
        }

        private void ExecuteActionAim(SkillData action, bool isAttack)
        {
            var canExecute = (!isAttack || Time.time > nextFire) && /*photonView.GetMana()*/mana >= action.MpCost;

            if (canExecute)
            {
                nextFire = Time.time + attackSpeed / 100f;

                var instantAim =
                    action.Aim == AimType.None ? transform.position :
                    action.Aim == AimType.WhileExecute ? transform.position + transform.forward :
                    Vector3.zero;

                if (action.Aim == AimType.None || action.Aim == AimType.WhileExecute)
                {
                    ExecuteActionInstantly(instantAim, null, isAttack);
                }
            }
        }

        private void ExecuteActionInstantly(Vector3 aimPosition, Player autoTarget, bool isAttack)
        {
            var action = isAttack ? attack : skill;

            AddMana(-action.MpCost);

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
            float targetTime = Time.time + 10;

            while (targetTime - Time.time > 0)
            {
                GameManager.GetInstance().ui.SetSpawnDelay(targetTime - Time.time);

                yield return null;
            }

            GameManager.GetInstance().ui.DisableDeath();

            photonView.RPC("RpcRevive", RpcTarget.All);
        }

        private void ToggleFunction(bool toggle)
        {
            rendererAnchor.SetActive(toggle);

            foreach (var collider in colliders)
            {
                collider.enabled = toggle;
            }
        }

        

        private IEnumerator YieldManaAutoRegen(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);

                AddMana(MaxMana / 10);
            }
        }

        #endregion
    }
}