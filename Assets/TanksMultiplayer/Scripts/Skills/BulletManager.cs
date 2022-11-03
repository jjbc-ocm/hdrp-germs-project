/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

namespace TanksMP
{
    public class BulletManager : SkillBaseManager
    {
        [SerializeField]
        private float speed = 10;

        [SerializeField]
        private AudioClip hitClip;

        [SerializeField]
        private AudioClip explosionClip;

        [SerializeField]
        private GameObject hitFX;

        [SerializeField]
        private GameObject explosionFX;
        

        #region Unity

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
                if (!IsHit(owner, player)) return;

                if (hitFX) PoolManager.Spawn(hitFX, transform.position, Quaternion.identity);
                if (hitClip) AudioManager.Play3D(hitClip, transform.position);
            }
            else if (monster != null)
            {
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

        protected override void OnInitialize()
        {
            rigidBody.velocity = transform.forward * speed;
        }
    }
}
