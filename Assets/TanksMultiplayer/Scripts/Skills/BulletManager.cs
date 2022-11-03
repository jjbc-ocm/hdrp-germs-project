/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

namespace TanksMP
{
    public class BulletManager : MonoBehaviour
    {
        [SerializeField]
        private float speed = 10;

        [SerializeField]
        private int damage = 3;

        [SerializeField]
        private float despawnDelay = 1f;

        //[SerializeField]
        //private int bounce = 0;

        //[SerializeField]
        //private int maxTargets = 1;

        //[SerializeField]
        //private float explosionRange = 1;

        [SerializeField]
        private AudioClip hitClip;

        [SerializeField]
        private AudioClip explosionClip;

        [SerializeField]
        private GameObject hitFX;

        [SerializeField]
        private GameObject explosionFX;

        private Rigidbody rigidBody;

        //private SphereCollider sphereCol;

        //private int maxBounce;

        //private Vector3 lastBouncePos;

        private Player owner;
        private GPMonsterBase monsterOwner;

        public int Damage { get => damage; }

        public Player Owner { get => owner; }
        public GPMonsterBase MonsterOwner { get => monsterOwner; } // TODO: make base class that the mosnters and players share

        /// <summary>
        /// True if the bullet belongs to a monster.
        /// </summary>
        public bool formMonster = false;

        /// <summary>
        /// Should the bullet ignore collisions with monsters?
        /// </summary>
        public bool ignoreMonsters = false;


        #region Unity

        void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag("IgnoreBullet")) // ignore collision
            {
                return;
            }

            //cache corresponding gameobject that was hit
            GameObject obj = col.gameObject;

            Player player = obj.GetComponent<Player>();
            GPMonsterBase monster = obj.GetComponent<GPMonsterBase>();

            if (player != null)
            {
                if (owner != null) // could be by a monster.
                {
                    if (!IsHit(owner, player)) return;
                }

                if (hitFX) PoolManager.Spawn(hitFX, transform.position, Quaternion.identity);
                if (hitClip) AudioManager.Play3D(hitClip, transform.position);
            }
            else if (monster != null)
            {
                if (ignoreMonsters)
                {
                    return;
                }
                //create clips and particles on hit
                if (hitFX) PoolManager.Spawn(hitFX, transform.position, Quaternion.identity);
                if (hitClip) AudioManager.Play3D(hitClip, transform.position);
            }


            //despawn gameobject
            //PoolManager.Despawn(gameObject);// TODO: handle object pooling in the future
            Destroy(gameObject);


            //the previous code is not synced to clients at all, because all that clients need is the
            //initial position and direction of the bullet to calculate the exact same behavior on their end.
            //at this point, continue with the critical game aspects only on the server
            if (!PhotonNetwork.IsMasterClient) return;

            //create list for affected players by this bullet and add the collided player immediately,
            //we have done validation & friendly fire checks above already
            List<Player> targets = new List<Player>();
            if(player != null) targets.Add(player);

            

            //apply bullet damage to the collided players
            for(int i = 0; i < targets.Count; i++)
            {
                targets[i].TakeDamage(this);
            }

            if (monster)
            {
                monster.DamageMonster(this);
            }
        }

        #endregion

        #region Public

        public void Initialize(Player owner)
        {
            this.owner = owner;

            rigidBody.velocity = transform.forward * speed;
        }

        public void Initialize(GPMonsterBase owner)
        {
            this.monsterOwner = owner;

            rigidBody.velocity = transform.forward * speed;
        }

        public void ChangeDirection(Vector3 direction)
        {
            transform.forward = direction;
            rigidBody.velocity = speed * direction;
        }

        #endregion

        #region Private

        private bool IsHit(Player origin, Player target)
        {
            if (target == owner || target == null) return false;

            else if (origin.photonView.GetTeam() == target.photonView.GetTeam()) return false;

            return true;
        }

        #endregion
    }
}
