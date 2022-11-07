using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

//[RequireComponent(typeof(BoxCollider))]
public class HolyBrightManager : SkillBaseManager
{
    [SerializeField]
    private float sustainDelay;

    //[SerializeField]
    //private float trailSpeed;

    //[SerializeField]
    //private GameObject vfx;

    //[SerializeField]
    //private GameObject prefabDebugBox; // TODO: delete once there's a VFX

    private float lastAttackTime;

    //private BoxCollider boxCollider;

    void Update()
    {
        transform.position = owner.transform.position;

        transform.eulerAngles = new Vector3(0, owner.transform.eulerAngles.y, 0);

        if (!PhotonNetwork.IsMasterClient) return;

        if (Time.time > lastAttackTime + sustainDelay)
        {
            lastAttackTime = Time.time;

            var center = transform.position + transform.forward * data.Range / 2f;

            var halfExtents = new Vector3(5, 5, data.Range) / 2f;

            var orientation = Quaternion.Euler(transform.eulerAngles);

            var colliders = Physics.OverlapBox(center, halfExtents, orientation, LayerMask.GetMask("Ship"));

            foreach (var collider in colliders)
            {
                var player = collider.GetComponent<Player>();

                if (!IsHit(owner, player)) continue;

                player.TakeDamage(this);
            }
        }
    }

    /*void OnTriggerEnter(Collider col)
    {
        Player player = col.GetComponent<Player>();
        
        if (!PhotonNetwork.IsMasterClient) return;

        if (!IsHit(owner, player)) return;

        player.TakeDamage(this);
    }*/

    protected override void OnInitialize()
    {
        //boxCollider = GetComponent<BoxCollider>();

        
        //StartCoroutine(YieldSpawnDebugBox());
    }

    private int spawnBoxCounter;

    /*private IEnumerator YieldSpawnDebugBox()
    {
        var spawnDelay = 0.1f;

        var trailSize = 0f;

        while (data.Range > trailSize)
        {
            yield return new WaitForSeconds(spawnDelay);

            trailSize = spawnBoxCounter * trailSpeed * spawnDelay;

            spawnBoxCounter++;

            boxCollider.size = new Vector3(5, 5, trailSize);

            boxCollider.center = boxCollider.size / 2f;


            // TODO: delete these part once there's a VFX
            //var position = transform.position + transform.forward * trailSize;

            *//*var rotation = Quaternion.Euler(
                Random.Range(0, 360),
                Random.Range(0, 360),
                Random.Range(0, 360));*//*

            //Instantiate(prefabDebugBox, position, rotation, transform);
        }
    }*/
}
