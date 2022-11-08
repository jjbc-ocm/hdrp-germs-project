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

namespace TanksMP
{
    public class Player : MonoBehaviourPunCallbacks, IPunObservable
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
        private GameObject aimIndicator;

        [SerializeField]
        private Collider[] colliders;

        [SerializeField]
        private GameObject rendererAnchor;

        [SerializeField]
        private GameObject iconIndicator;

        [HideInInspector]
        public FollowTarget camFollow;

        private Rigidbody rigidBody;

        private float nextFire;

        private Vector2 moveDir;

        private Vector2 prevMoveDir;

        private bool isExecutingActionAim;

        private Player aimAutoTarget;

        [Header("Events")]
        public UnityEvent<int> onDieEvent;

        #region Network Sync

        private Vector3 shipRotation;

        private bool isRespawning;

        #endregion

        

        public int MaxHealth { get => maxHealth; }

        public int MaxMana { get => maxMana; }

        public Sprite SpriteIcon { get => spriteIcon; }

        public GameObject IconIndicator { get => iconIndicator; }

        public bool IsRespawning { get => isRespawning; }




        #region Unity

        void Awake()
        {
            if(!PhotonNetwork.IsMasterClient)
                return;
            
            photonView.SetHealth(maxHealth);

            photonView.SetMana(maxMana);

            StartCoroutine(YieldManaAutoRegen(1));
        }

        void Start()
        {
            //called only for this client 
            if (!photonView.IsMine)
                return;

			//set a global reference to the local player
            GameManager.GetInstance().localPlayer = this;

            //get components and set camera target
            rigidBody = GetComponent<Rigidbody>();
            camFollow = GameManager.GetInstance().mainCamera.GetComponent<FollowTarget>();
            camFollow.target = transform;
        }

        void Update()
        {
            if (!photonView.IsMine) return;

            /* Update skills */
            if (Input.GetMouseButton(0))
            {
                if (!isExecutingActionAim)
                {
                    ExecuteActionAim(attack, true);
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                ExecuteActionAim(skill, false);
            }

            if (Input.GetMouseButtonUp(1) && isExecutingActionAim)
            {
                ExecuteActionInstantly(aimIndicator.transform.position, aimAutoTarget, false);
            }

            if (isExecutingActionAim)
            {
                var action = skill;

                aimIndicator.SetActive(true);

                var ray = GameManager.GetInstance().mainCamera.ScreenPointToRay(Input.mousePosition);

                var layerName = action.Aim == AimType.Water ? "Water" : "Ship";

                if (Physics.Raycast(ray, out RaycastHit hit, action.Range, LayerMask.GetMask(layerName)))
                {
                    if (action.Aim == AimType.Water ||
                        action.Aim == AimType.AnyShip ||
                        (action.Aim == AimType.EnemyShip && IsEnemyShip(hit)) ||
                        (action.Aim == AimType.AllyShip && !IsEnemyShip(hit)))
                    {
                        aimIndicator.transform.position = hit.point;

                        if (action.Aim == AimType.EnemyShip || 
                            action.Aim == AimType.AllyShip ||
                            action.Aim == AimType.AnyShip)
                        {
                            aimAutoTarget = hit.transform.GetComponent<Player>();
                        }
                    }
                }
            }
            else
            {
                aimIndicator.SetActive(false);
            }
        }

        void FixedUpdate()
        {
            /* Make sure the only thing we can control is only our ship */
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

        #endregion

        #region Photon

        void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(shipRotation);
                stream.SendNext(isRespawning);
            }
            else
            {
                shipRotation = (Vector3)stream.ReceiveNext();
                isRespawning = (bool)stream.ReceiveNext();
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
        public void RpcDestroy(short attackerId)
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
            isRespawning = false;

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

        #endregion

        #region Public

        public void TakeDamage(SkillBaseManager action)
        {
            var bullet = action as BulletManager;

            var health = photonView.GetHealth();

            health -= action.Damage;

            if (health <= 0)
            {
                if (GameManager.GetInstance().IsGameOver()) return;

                if (bullet == null || !bullet.formMonster)
                {
                    /* Add score to opponent */
                    var attcker = action.Owner;
                    var attackerTeam = attcker.photonView.GetTeam();

                    if (photonView.GetTeam() != attackerTeam)
                    {
                        GameManager.GetInstance().AddScore(ScoreType.Kill, attackerTeam);
                        GPRewardSystem.m_instance.AddGoldToPlayer(attcker.photonView.Owner, "Kill");
                    }

                    /* Set game over from conditions */
                    if (GameManager.GetInstance().IsGameOver())
                    {
                        PhotonNetwork.CurrentRoom.IsOpen = false;

                        photonView.RPC("RpcGameOver", RpcTarget.All, (byte)attackerTeam);

                        return;
                    }
                }

                /* Handle collectible drops */
                var collectibles = GetComponentsInChildren<Collectible>(true);

                for (int i = 0; i < collectibles.Length; i++)
                {
                    // TODO: need to handle dropping of chest here
                    //PhotonNetwork.RemoveRPCs(collectibles[i].spawner.photonView);
                    //collectibles[i].spawner.photonView.RPC("Drop", RpcTarget.AllBuffered, transform.position);
                }

                /* Reset health, prepare for their respawn */
                photonView.SetHealth(maxHealth);

                short attackerId = 0;

                if (action.Owner)
                {
                    attackerId = (short)action.Owner.GetComponent<PhotonView>().ViewID;
                }

                if (bullet != null && bullet.formMonster)
                {
                    attackerId = -1;
                }

                photonView.RPC("RpcDestroy", RpcTarget.All, (short)attackerId);
            }
            else
            {
                photonView.SetHealth(health);
            }
        }

        /// <summary>
        /// Server only: calculate damage to be taken by the Player from a monster,
		/// triggers respawn workflow on death.
        /// </summary>
        public void TakeMonsterDamage(GPMonsterBase monster)
        {
            var health = photonView.GetHealth();

            health -= monster.m_damagePoints;

            if (health <= 0)
            {
                if (GameManager.GetInstance().IsGameOver()) return;

                /* Reset health, prepare for their respawn */
                photonView.SetHealth(maxHealth);

                photonView.RPC("RpcDestroy", RpcTarget.All, (short)-1);
            }
            else
            {
                photonView.SetHealth(health);
            }
        }
        
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

                transform.rotation = Quaternion.LookRotation(forward) * Quaternion.Euler(0, camFollow.camTransform.eulerAngles.y, 0);
            }

            var acceleration = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? 2 : 1;

            Vector3 moveForce = transform.forward * direction.y * moveSpeed * acceleration;

            rigidBody.AddForce(moveForce);
        }

        private void ExecuteActionAim(SkillData action, bool isAttack)
        {
            var canExecute = (!isAttack || Time.time > nextFire) && photonView.GetMana() >= action.MpCost;

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
                else
                {
                    isExecutingActionAim = true;
                }
            }
        }

        private void ExecuteActionInstantly(Vector3 aimPosition, Player autoTarget, bool isAttack)
        {
            var action = isAttack ? attack : skill;

            isExecutingActionAim = false;

            photonView.SetMana(photonView.GetMana() - action.MpCost);

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

            float targetTime = Time.time + 3;

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

        private bool IsEnemyShip(RaycastHit hit)
        {
            var player = hit.transform.GetComponent<Player>();

            return !player || (player && player.photonView.GetTeam() != photonView.GetTeam());
        }

        private IEnumerator YieldManaAutoRegen(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);

                photonView.SetMana(Mathf.Min(photonView.GetMana() + MaxMana / 10, MaxMana));
            }
        }

        #endregion
    }
}