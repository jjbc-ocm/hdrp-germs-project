using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TrailOfFrostManager : SkillBaseManager
{
    [SerializeField]
    private float expandTime;



    protected override void OnInitialize()
    {

    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider col)
    {
        /*if (col.CompareTag("IgnoreBullet")) // ignore collision
        {
            return;
        }*/

        //cache corresponding gameobject that was hit
        GameObject obj = col.gameObject;

        Player player = obj.GetComponent<Player>();
        //GPMonsterBase monster = obj.GetComponent<GPMonsterBase>();

        /*if (player != null)
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
        }*/

        if (!PhotonNetwork.IsMasterClient) return;

        if (!IsHit(owner, player)) return;

        player.TakeDamage(this);

        //create list for affected players by this bullet and add the collided player immediately,
        //we have done validation & friendly fire checks above already
        /*List<Player> targets = new List<Player>();
        if (player != null) targets.Add(player);



        //apply bullet damage to the collided players
        for (int i = 0; i < targets.Count; i++)
        {
            targets[i].TakeDamage(this);
        }

        if (monster)
        {
            monster.DamageMonster(this);
        }*/
    }
}
