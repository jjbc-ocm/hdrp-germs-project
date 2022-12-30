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
        private float speed;

        [SerializeField]
        private AudioClip hitClip;

        [SerializeField]
        private AudioClip explosionClip;

        [SerializeField]
        private GameObject hitFX;

        [SerializeField]
        private GameObject explosionFX;

        private Rigidbody rigidBody;


        #region Unity

        void Start()
        {
            Destroy(gameObject, Constants.FOG_OF_WAR_DISTANCE / speed * 0.9f);
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag("IgnoreBullet")) // ignore collision
            {
                return;
            }

            GameObject obj = col.gameObject;

            var target = obj.GetComponent<ActorManager>();

            if (!IsHit(owner, target)) return;

            if (hitFX) PoolManager.Spawn(hitFX, transform.position, Quaternion.identity);
            if (hitClip) AudioManager.Play3D(hitClip, transform.position);

            Destroy(gameObject);


            //the previous code is not synced to clients at all, because all that clients need is the
            //initial position and direction of the bullet to calculate the exact same behavior on their end.
            //at this point, continue with the critical game aspects only on the server
            if (!PhotonNetwork.IsMasterClient) return;

            //create list for affected players by this bullet and add the collided player immediately,
            //we have done validation & friendly fire checks above already
            List<ActorManager> targets = new List<ActorManager>();
            if(target != null) targets.Add(target);

            //apply bullet damage to the collided players
            for (int i = 0; i < targets.Count; i++)
            {
                targets[i].photonView.RPC("RpcDamageHealth", RpcTarget.All, damage, owner.photonView.ViewID);

                if (owner is Player)
                {
                    var lifeSteal = -Mathf.Max(1, Mathf.RoundToInt(damage * (owner as Player).Inventory.StatModifier.LifeSteal));

                    owner.photonView.RPC("RpcDamageHealth", RpcTarget.All, lifeSteal, 0);
                }
            }
        }

        #endregion

        protected override void OnInitialize()
        {
            rigidBody = GetComponent<Rigidbody>();

            rigidBody.velocity = transform.forward * speed;
        }
    }
}
