/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;
using UnityEngine.UI;
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
        private GameObject bullet;

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

        void FixedUpdate()
        { 
            if (!photonView.IsMine)
            {
                return;
            }

            Vector2 turnDir;

            //reset moving input when no arrow keys are pressed down
            if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            {
                moveDir.x += (0 - prevMoveDir.x) * 0.1f;
                moveDir.y += (0 - prevMoveDir.y) * 0.1f;
            }
            else
            {
                //read out moving directions and calculate force
                moveDir.x += (Input.GetAxis("Horizontal") - prevMoveDir.x) * 0.1f;
                moveDir.y += (Input.GetAxis("Vertical") - prevMoveDir.y) * 0.1f;
                Move(moveDir);
            }

            //cast a ray on a plane at the mouse position for detecting where to shoot 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.up);
            float distance = 0f;
            Vector3 hitPos = Vector3.zero;
            //the hit position determines the mouse position in the scene
            if (plane.Raycast(ray, out distance))
            {
                hitPos = ray.GetPoint(distance) - transform.position;
            }

            //we've converted the mouse position to a direction
            turnDir = new Vector2(hitPos.x, hitPos.z);

            //rotate turret to look at the mouse direction
            //RotateTurret(new Vector2(hitPos.x, hitPos.z));

            shipRotation = new Vector3(
                moveDir.y * -10,
                rendererAnchor.transform.localEulerAngles.y,
                moveDir.x * -10);

            rendererAnchor.transform.localRotation = Quaternion.Euler(shipRotation);
            

            if (Input.GetButton("Fire1"))
                Shoot();

            prevMoveDir = moveDir;

            //replicate input to mobile controls for illustration purposes
#if UNITY_EDITOR
            //GameManager.GetInstance().ui.controls[0].position = moveDir;
            //GameManager.GetInstance().ui.controls[1].position = turnDir;
#endif
        }

        #endregion

        /// <summary>
        /// This method gets called whenever player properties have been changed on the network.
        /// </summary>
        /// TODO: this is might not needed because player HUD is always updated every frames
        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player player, ExitGames.Client.Photon.Hashtable playerAndUpdatedProps)
        {
            //only react on property changes for this player
            if(player != photonView.Owner)
                return;

            //update values that could change any time for visualization to stay up to date
            //OnHealthChange(player.GetHealth());
            //OnShieldChange(player.GetShield());
        }

        
        //this method gets called multiple times per second, at least 10 times or more
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

        private Vector2 prevMoveDir;

        

        void Move(Vector2 direction = default(Vector2))
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


        

        //shoots a bullet in the direction passed in
        //we do not rely on the current turret rotation here, because we send the direction
        //along with the shot request to the server to absolutely ensure a synced shot position
        protected void Shoot(Vector2 direction = default(Vector2))
        {
            //if shot delay is over  
            if (Time.time > nextFire)
            {
                //set next shot timestamp
                nextFire = Time.time + attackSpeed / 100f;

                //send current client position and turret rotation along to sync the shot position
                //also we are sending it as a short array (only x,z - skip y) to save additional bandwidth
                float[] pos = new float[] { transform.position.x , transform.position.z };
                //send shot request with origin to server
                this.photonView.RPC("CmdShoot", RpcTarget.AllViaServer, pos, (short)transform.eulerAngles.y);
            }
        }
        
        
        //called on the server first but forwarded to all clients
        [PunRPC]
        public void CmdShoot(float[] position, short angle)
        {
            //calculate center between shot position sent and current server position (factor 0.6f = 40% client, 60% server)
            //this is done to compensate network lag and smoothing it out between both client/server positions
            Vector3 shotCenter = Vector3.Lerp(transform.position, new Vector3(position[0], transform.position.y, position[1]), 0.6f);
            Quaternion syncedRot = Quaternion.Euler(0, angle, 0);

            //spawn bullet using pooling
            GameObject obj = PoolManager.Spawn(bullet, shotCenter, syncedRot);
            obj.GetComponent<Bullet>().owner = gameObject;

            var trails = obj.GetComponentsInChildren<TrailRenderer>();

            foreach (var trail in trails)
            {
                trail.Clear();
            }

            //send event to all clients for spawning effects
            if (shotFX || shotClip)
                RpcOnShot();
        }


        //called on all clients after bullet spawn
        //spawn effects or sounds locally, if set
        protected void RpcOnShot()
        {
            if (shotFX) PoolManager.Spawn(shotFX, transform.position, Quaternion.identity);
            if (shotClip) AudioManager.Play3D(shotClip, transform.position, 0.1f);
        }

        /// <summary>
        /// Server only: calculate damage to be taken by the Player,
		/// triggers score increase and respawn workflow on death.
        /// </summary>
        public void TakeDamage(Bullet bullet)
        {
            var health = photonView.GetHealth();

            health -= bullet.damage;

            if (health <= 0)
            {
                if (GameManager.GetInstance().IsGameOver()) return;

                if (!bullet.formMonster)
                {
                    /* Add score to opponent */
                    var attcker = bullet.owner.GetComponent<Player>();

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

                var attackerId = (short)bullet.owner.GetComponent<PhotonView>().ViewID;

                if (bullet.formMonster)
                {
                    attackerId = (short)-1;

                    GPMonsterBase monster = bullet.owner.GetComponent<GPMonsterBase>();
                    if (monster)
                    {
                        monster.OnPlayerKilled(this);
                    }
                }

                photonView.RPC("RpcDestroy", RpcTarget.All, attackerId);
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
                monster.OnPlayerKilled(this);
            }
            else
            {
                photonView.SetHealth(health);
            }
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
                Debug.Log("RpcDestroy A");

                if (attackerView != null)
                {
                    camFollow.target = attackerView.transform;
                }

                Debug.Log("RpcDestroy B");

                camFollow.HideMask(true);

                Debug.Log("RpcDestroy C");

                if (attackerView != null)
                {
                    GameManager.GetInstance().ui.SetDeathText(
                    attackerView.GetName(),
                    GameManager.GetInstance().teams[attackerView.GetTeam()]);
                }

                Debug.Log("RpcDestroy D");

                //GameManager.GetInstance().SpawnPlayer(photonView);
                StartCoroutine(SpawnRoutine());
            }
        }

        private IEnumerator SpawnRoutine()
        {
            Debug.Log("SpawnRoutine A");

            float targetTime = Time.time + 3;

            while (targetTime - Time.time > 0)
            {
                GameManager.GetInstance().ui.SetSpawnDelay(targetTime - Time.time);

                yield return null;
            }

            Debug.Log("SpawnRoutine B");

            GameManager.GetInstance().ui.DisableDeath();

            Debug.Log("SpawnRoutine C");

            photonView.RPC("RpcRevive", RpcTarget.All);
        }

        [PunRPC]
        public void RpcRevive()
        {
            Debug.Log("RpcRespawn A");

            ToggleFunction(true);

            Debug.Log("RpcRespawn B");

            if (photonView.IsMine)
            {
                Debug.Log("RpcRespawn C");

                ResetPosition();

                Debug.Log("RpcRespawn D");
            }
        }

        private void ToggleFunction(bool toggle)
        {
            rendererAnchor.SetActive(toggle);

            foreach (var collider in colliders)
            {
                collider.enabled = toggle;
            }
        }


        /// <summary>
        /// Repositions in team area and resets camera & input variables.
        /// This should only be called for the local player.
        /// </summary>
        public void ResetPosition()
        {
            //start following the local player again
            camFollow.target = transform;
            camFollow.HideMask(false);

            //get team area and reposition it there
            transform.position = GameManager.GetInstance().GetSpawnPosition(photonView.GetTeam());

            //reset forces modified by input
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
            transform.rotation = Quaternion.identity;
            //reset input left over
            //GameManager.GetInstance().ui.controls[0].OnEndDrag(null);
            //GameManager.GetInstance().ui.controls[1].OnEndDrag(null);
        }


        //called on all clients on game end providing the winning team
        [PunRPC]
        public void RpcGameOver(byte teamIndex)
        {
            //display game over window
            GameManager.GetInstance().DisplayGameOver(teamIndex);
        }
    }
}