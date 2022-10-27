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
        //[SerializeField]
        //private Text label;

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


        [Header("Other Properties")]

        /// <summary>
        /// Clip to play when a shot has been fired.
        /// </summary>
        public AudioClip shotClip;

        /// <summary>
        /// Clip to play on player death.
        /// </summary>
        public AudioClip explosionClip;

        /// <summary>
        /// Object to spawn on shooting.
        /// </summary>
        public GameObject shotFX;

        /// <summary>
        /// Object to spawn on player death.
        /// </summary>
        public GameObject explosionFX;

        /// <summary>
        /// Turret to rotate with look direction.
        /// </summary>
        public Transform turret;

        /// <summary>
        /// Position to spawn new bullets at.
        /// </summary>
        public Transform shotPos;

        /// <summary>
        /// Array of available bullets for shooting.
        /// </summary>
        public GameObject[] bullets;

        /// <summary>
        /// MeshRenderers that should be highlighted in team color.
        /// </summary>
        public MeshRenderer[] renderers;

        [SerializeField]
        private Collider[] colliders;

        public GameObject ship;

        public Vector2 moveDir;

        public GameObject iconIndicator;

        /// <summary>
        /// Last player gameobject that killed this one.
        /// </summary>
        [HideInInspector]
        public GameObject killedBy;

        /// <summary>
        /// Reference to the camera following component.
        /// </summary>
        [HideInInspector]
        public FollowTarget camFollow;

        //timestamp when next shot should happen
        private float nextFire;
        
        //reference to this rigidbody
        #pragma warning disable 0649
		private Rigidbody rb;
#pragma warning restore 0649

        private bool isToggled;

        //public Text Label { get => label; }

        public int MaxHealth { get => maxHealth; }

        public int MaxMana { get => maxMana; }


        #region Network Sync

        protected short turretRotation;

        protected Vector3 shipRotation;

        #endregion

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
			//get corresponding team and colorize renderers in team color
            //Team team = GameManager.GetInstance().teams[photonView.GetTeam()];
            //for(int i = 0; i < renderers.Length; i++)
            //    renderers[i].material = team.material;

            //set name in label
            //label.text = photonView.GetName();
            //call hooks manually to update
            //OnHealthChange(photonView.GetHealth());
            //OnShieldChange(photonView.GetShield());

            //called only for this client 
            if (!photonView.IsMine)
                return;

			//set a global reference to the local player
            GameManager.GetInstance().localPlayer = this;

			//get components and set camera target
            rb = GetComponent<Rigidbody>();
            camFollow = GameManager.GetInstance().mainCamera.GetComponent<FollowTarget>();//Camera.main.GetComponent<FollowTarget>();
            camFollow.target = transform;
        }

        void FixedUpdate()
        {
            //skip further calls for remote clients    
            if (!photonView.IsMine)
            {
                //keep turret rotation updated for all clients
                OnTurretRotation();
                return;
            }

            //movement variables
            //moveDir = Vector2.zero;

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
            RotateTurret(new Vector2(hitPos.x, hitPos.z));

            //rotate ship based on turnDir
            //TODO: must put this in 
            shipRotation = new Vector3(
                moveDir.y * -10,//ship.transform.localEulerAngles.x, 
                ship.transform.localEulerAngles.y,
                moveDir.x * -10);
            ship.transform.localRotation = Quaternion.Euler(shipRotation);
            //rb.AddTorque(new Vector3(0, 0, moveDir.x * 15f));

            //shoot bullet on left mouse click
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
                //here we send the turret rotation angle to other clients
                stream.SendNext(turretRotation);
                stream.SendNext(shipRotation);
            }
            else
            {   
                //here we receive the turret rotation angle from others and apply it
                turretRotation = (short)stream.ReceiveNext();
                shipRotation = (Vector3)stream.ReceiveNext();
                OnTurretRotation();

                ship.transform.localRotation = Quaternion.Euler(this.shipRotation);
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

            rb.AddForce(moveForce);
        }


        //on movement drag ended
        void MoveEnd()
        {
            //reset rigidbody physics values
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }


        //rotates turret to the direction passed in
        // Commented by: Jilmer John Cariaso
        /*void RotateTurret(Vector2 direction = default(Vector2))
        {
            //don't rotate without values
            if (direction == Vector2.zero)
                return;

            //get rotation value as angle out of the direction we received
            turretRotation = (short)Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y)).eulerAngles.y;
            OnTurretRotation();
        }*/
        void RotateTurret(Vector2 direction = default(Vector2))
        {
            //don't rotate without values
            if (direction == Vector2.zero)
                return;

            //get rotation value as angle out of the direction we received
            turretRotation = (short)Quaternion.LookRotation(transform.forward).eulerAngles.y;
            OnTurretRotation();
        }


        //on shot drag start set small delay for first shot
        void ShootBegin()
        {
            nextFire = Time.time + 0.25f;
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
                float[] pos = new float[] { shotPos.position.x , shotPos.position.z };
                //send shot request with origin to server
                this.photonView.RPC("CmdShoot", RpcTarget.AllViaServer, pos, turretRotation);
            }
        }
        
        
        //called on the server first but forwarded to all clients
        [PunRPC]
        public void CmdShoot(float[] position, short angle)
        {
            //get current bullet type
            int currentBullet = 0; // photonView.GetBullet();

            //calculate center between shot position sent and current server position (factor 0.6f = 40% client, 60% server)
            //this is done to compensate network lag and smoothing it out between both client/server positions
            Vector3 shotCenter = Vector3.Lerp(shotPos.position, new Vector3(position[0], shotPos.position.y, position[1]), 0.6f);
            Quaternion syncedRot = turret.rotation = Quaternion.Euler(0, angle, 0);

            //spawn bullet using pooling
            GameObject obj = PoolManager.Spawn(bullets[currentBullet], shotCenter, syncedRot);
            obj.GetComponent<Bullet>().owner = gameObject;

            //check for current ammunition
            //let the server decrease special ammunition, if present
            if (PhotonNetwork.IsMasterClient && currentBullet != 0)
            {
                //if ran out of ammo: reset bullet automatically
                //photonView.DecreaseAmmo(1);
            }

            //send event to all clients for spawning effects
            if (shotFX || shotClip)
                RpcOnShot();
        }


        //called on all clients after bullet spawn
        //spawn effects or sounds locally, if set
        protected void RpcOnShot()
        {
            if (shotFX) PoolManager.Spawn(shotFX, shotPos.position, Quaternion.identity);
            if (shotClip) AudioManager.Play3D(shotClip, shotPos.position, 0.1f);
        }


        //hook for updating turret rotation locally
        void OnTurretRotation()
        {
            //we don't need to check for local ownership when setting the turretRotation,
            //because OnPhotonSerializeView PhotonStream.isWriting == true only applies to the owner
            turret.rotation = Quaternion.Euler(0, turretRotation, 0);
        }


        //hook for updating health locally
        //(the actual value updates via player properties)
       /* protected void OnHealthChange(int value)
        {
            //healthSlider.value = (float)value / maxHealth;
        }*/


        //hook for updating shield locally
        //(the actual value updates via player properties)
        /*protected void OnShieldChange(int value)
        {
            //shieldSlider.value = value;
        }*/


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

                /* Add score to opponent */
                var attcker = bullet.owner.GetComponent<Player>();

                var attackerTeam = attcker.photonView.GetTeam();

                if(photonView.GetTeam() != attackerTeam)
                {
                    GameManager.GetInstance().AddScore(ScoreType.Kill, attackerTeam);
                }

                /* Set game over from conditions */
                if(GameManager.GetInstance().IsGameOver())
                {
                    PhotonNetwork.CurrentRoom.IsOpen = false;

                    photonView.RPC("RpcGameOver", RpcTarget.All, (byte)attackerTeam);

                    return;
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

                photonView.RPC("RpcDestroy", RpcTarget.All, attackerId);
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

                GameManager.GetInstance().ui.SetDeathText(
                    attackerView.GetName(), 
                    GameManager.GetInstance().teams[attackerView.GetTeam()]);

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

            photonView.RPC("RpcRespawn", RpcTarget.All);
        }

        [PunRPC]
        public void RpcRespawn()
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
            isToggled = toggle;

            foreach (var renderer in renderers)
            {
                renderer.enabled = toggle;
            }

            foreach (var collider in colliders)
            {
                collider.enabled = toggle;
            }
        }

        //called on all clients on both player death and respawn
        //only difference is that on respawn, the client sends the request
        /*[PunRPC]
        protected virtual void RpcRespawnOld(short senderId)
        {
            //toggle visibility for player gameobject (on/off)
            gameObject.SetActive(!gameObject.activeInHierarchy);
            bool isActive = gameObject.activeInHierarchy;
            killedBy = null;

            //the player has been killed
            if (!isActive)
            {
                //find original sender game object (killedBy)
                PhotonView senderView = senderId > 0 ? PhotonView.Find(senderId) : null;
                if (senderView != null && senderView.gameObject != null) killedBy = senderView.gameObject;

                //detect whether the current user was responsible for the kill, but not for suicide
                //yes, that's my kill: increase local kill counter
                if (this != GameManager.GetInstance().localPlayer && killedBy == GameManager.GetInstance().localPlayer.gameObject)
                {
                    //GameManager.GetInstance().ui.killCounter[0].text = (int.Parse(GameManager.GetInstance().ui.killCounter[0].text) + 1).ToString();
                    //GameManager.GetInstance().ui.killCounter[0].GetComponent<Animator>().Play("Animation");
                }

                if (explosionFX)
                {
                    //spawn death particles locally using pooling and colorize them in the player's team color
                    GameObject particle = PoolManager.Spawn(explosionFX, transform.position, transform.rotation);
                    ParticleColor pColor = particle.GetComponent<ParticleColor>();
                    if (pColor) pColor.SetColor(GameManager.GetInstance().teams[photonView.GetTeam()].material.color);
                }

                //play sound clip on player death
                if (explosionClip) AudioManager.Play3D(explosionClip, transform.position);
            }

            if (PhotonNetwork.IsMasterClient)
            {
                //send player back to the team area, this will get overwritten by the exact position from the client itself later on
                //we just do this to avoid players "popping up" from the position they died and then teleporting to the team area instantly
                //this is manipulating the internal PhotonTransformView cache to update the networkPosition variable
                GetComponent<PhotonTransformView>().OnPhotonSerializeView(new PhotonStream(false, new object[] { GameManager.GetInstance().GetSpawnPosition(photonView.GetTeam()),
                                                                                                                 Vector3.zero, Quaternion.identity }), new PhotonMessageInfo());
            }

            //further changes only affect the local client
            if (!photonView.IsMine)
                return;

            //local player got respawned so reset states
            if (isActive == true)
                ResetPosition();
            else
            {
                //local player was killed, set camera to follow the killer
                if (killedBy != null) camFollow.target = killedBy.transform;
                //hide input controls and other HUD elements
                camFollow.HideMask(true);
                //display respawn window (only for local player)
                GameManager.GetInstance().DisplayDeath();
            }
        }*/


        /// <summary>
        /// Command telling the server and all others that this client is ready for respawn.
        /// This is when the respawn delay is over or a video ad has been watched.
        /// </summary>
        /*public void CmdRespawn()
        {
            this.photonView.RPC("RpcRespawn", RpcTarget.AllViaServer, (short)0);
        }*/


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
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
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