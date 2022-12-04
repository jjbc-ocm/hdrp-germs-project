using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TrailOfFrostManager : SkillBaseManager
{
    [SerializeField]
    private float trailSpeed;

    [SerializeField]
    private float spawnDelay;

    [SerializeField]
    private float spawnOffsetDown;

    [SerializeField]
    private float spawnOffsetForward;

    [SerializeField]
    private GameObject prefabIcicle;

    private BoxCollider boxCollider;

    void OnTriggerEnter(Collider col)
    {
        var actor = col.GetComponent<ActorManager>();

        Debug.Log("SDFGSFDFG A");
        
        if (!PhotonNetwork.IsMasterClient) return;

        Debug.Log("SDFGSFDFG B");

        if (!IsHit(owner, actor)) return;

        Debug.Log("SDFGSFDFG C");

        actor.photonView.RPC("RpcDamageHealth", RpcTarget.All, damage, owner.photonView.ViewID);
    }

    protected override void OnInitialize()
    {
        boxCollider = GetComponent<BoxCollider>();

        
        StartCoroutine(YieldSpawnDebugBox());
    }

    private int spawnBoxCounter;

    private IEnumerator YieldSpawnDebugBox()
    {
        var trailSize = 0f;

        while (data.Range > trailSize)
        {
            yield return new WaitForSeconds(spawnDelay);

            trailSize = spawnBoxCounter * trailSpeed * spawnDelay;

            spawnBoxCounter++;

            boxCollider.size = new Vector3(5, 50, trailSize);

            boxCollider.center = boxCollider.size / 2f;

            var position = transform.position + transform.forward * trailSize;

            var offset = new Vector3(
                spawnOffsetForward * transform.forward.x,
                -spawnOffsetDown,
                spawnOffsetForward * transform.forward.z);

            var rotation = Quaternion.identity;

            Instantiate(prefabIcicle, position + offset, rotation, transform);
        }
    }
}
