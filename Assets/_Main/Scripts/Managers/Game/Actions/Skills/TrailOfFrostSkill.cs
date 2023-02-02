using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TrailOfFrostSkill : SkillBase
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

    void OnTriggerEnter(Collider col)
    {
        var actor = col.GetComponent<ActorManager>();

        if (!PhotonNetwork.IsMasterClient) return;

        if (!IsHit(owner, actor)) return;

        ApplyEffect(owner, actor);
    }

    protected override void OnInitialize()
    {
        StartCoroutine(YieldSpawnIcicle());
    }

    private int spawnBoxCounter;

    private IEnumerator YieldSpawnIcicle()
    {
        var boxCollider = GetComponent<BoxCollider>();

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

            AudioManager.Instance.Play3D(data.Sounds[0], position + offset);
        }
    }
}
