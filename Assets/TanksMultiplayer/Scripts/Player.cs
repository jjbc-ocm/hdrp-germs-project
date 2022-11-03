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

        private bool isExecutingActionAttack;

        [Header("Events")]
        public UnityEvent<int> onDieEvent;

        #region Network Sync

        private Vector3 shipRotation;

        #endregion

        

        public int MaxHealth { get => maxHealth; }

        public int MaxMana { get => maxMana; }

        public Sprite SpriteIcon { get => spriteIcon; }

        public GameObject IconIndicator { get => iconIndicator; }


        

        #region Unity

        void Awake()
        {
            if(!PhotonNetwork.IsMasterClient)
                return;
            
            photonView.SetHealth(maxHealth);

            photonView.SetMana(maxMana);
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
            if (isExecutingActionAim)
            {
                var action = isExecutingActionAttack ? attack : skill;

                if (action.Aim == AimType.Water)
                {
                    aimIndicator.SetActive(true);

                    var ray = GameManager.GetInstance().mainCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out RaycastHit hit, action.Range, LayerMask.GetMask("Water")))
                    {
                        aimIndicator.transform.position = hit.point;
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

            /* Update skills */
            if (Input.GetButton("Fire1"))
            {
                if (isExecutingActionAim)
                {
                    ExecuteActionAimSelection(isExecutingActionAttack);
                }
                else
                {
                    ExecuteAction(attack, true);
                }
            }
                

            if(Input.GetButton("Fire2"))
            {
                ExecuteAction(skill, false);
            }

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

        /*[PunRPC] // TODO: not used, delete later
        public void RpcAttack(float[] position, short angle)
        {
            //calculate center between shot position sent and current server position (factor 0.6f = 40% client, 60% server)
            //this is done to compensate network lag and smoothing it out between both client/server positions
            Vector3 shotCenter = Vector3.Lerp(transform.position, new Vector3(position[0], transform.position.y, position[1]), 0.6f);
            Quaternion syncedRot = Quaternion.Euler(0, angle, 0);

            //spawn bullet using pooling
            GameObject obj = PoolManager.Spawn(attack.Effect, shotCenter, syncedRot);
            obj.GetComponent<BulletManager>().owner = gameObject;

            var trails = obj.GetComponentsInChildren<TrailRenderer>();

            foreach (var trail in trails)
            {
                trail.Clear();
            }

            if (shotFX) PoolManager.Spawn(shotFX, transform.position, Quaternion.identity);
            if (shotClip) AudioManager.Play3D(shotClip, transform.position, 0.1f);
        }*/

        [PunRPC]
        public void RpcAction(float[] position, float[] target, bool isAttack)
        {
            var action = isAttack ? attack : skill;

            /* Steps
             * 1. Calculate the rotation ased on position and target
             * 2. Spawn action.Effect based on position, and rotation
             * 3. pass the action (SkillData) as parameter to the spawned object
             * 4. Any trail reset or sound effects should be done on the actual object spawned
             */
            var vPosition = new Vector3(position[0], position[1], position[2]);

            var vTarget = new Vector3(target[0], target[1], target[2]);

            var forward = vTarget - vPosition;

            var rotation = Quaternion.LookRotation(forward);

            var effect = action.IsSpawnOnAim
                ? Instantiate(action.Effect, vTarget, rotation)
                : Instantiate(action.Effect, vPosition, rotation);

            effect.Initialize(this); // TODO: 3 and 4
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

        private void ExecuteAction(SkillData action, bool isAttack)
        {
            var canExecute = Time.time > nextFire && photonView.GetMana() >= action.MpCost;

            if (canExecute)
            {
                nextFire = Time.time + attackSpeed / 100f;

                var instantAim =
                    action.Aim == AimType.None ? transform.position :
                    action.Aim == AimType.WhileExecute ? transform.position + transform.forward :
                    Vector3.zero;

                if (action.Aim == AimType.None || action.Aim == AimType.WhileExecute)
                {
                    ExecuteActionInstantly(instantAim, isAttack);
                }
                else
                {
                    isExecutingActionAim = true;

                    isExecutingActionAttack = isAttack;
                }
            }
        }

        private void ExecuteActionInstantly(Vector3 aimPosition, bool isAttack)
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
                isAttack);
        }

        private void ExecuteActionAimSelection(bool isAttack)
        {
            var action = isAttack ? attack : skill;

            ExecuteActionInstantly(aimIndicator.transform.position, isAttack);
        }

        private IEnumerator SpawnRoutine()
        {
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

        #endregion
    }
}