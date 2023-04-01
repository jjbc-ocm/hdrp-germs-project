using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

//[RequireComponent(typeof(BoxCollider))]
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

    protected override void OnInitialize()
    {
        StartCoroutine(YieldSpawnIcicle());

        AudioManager.Instance.Play3D(data.Sounds[0], transform.position);
    }

    private int spawnBoxCounter;

    private IEnumerator YieldSpawnIcicle()
    {
        //var boxCollider = GetComponent<BoxCollider>();

        var trailSize = 0f;

        while (data.Range > trailSize)
        {
            yield return new WaitForSeconds(spawnDelay);

            trailSize = spawnBoxCounter * trailSpeed * spawnDelay;

            spawnBoxCounter++;

            var position = transform.position + transform.forward * trailSize;

            var offset = new Vector3(
                spawnOffsetForward * transform.forward.x,
                -spawnOffsetDown,
                spawnOffsetForward * transform.forward.z);

            var rotation = Quaternion.identity;

            Instantiate(prefabIcicle, position + offset, rotation, transform);

            ExecuteDamageOnCollision(position);
        }
    }

    private void ExecuteDamageOnCollision(Vector3 position)
    {
        var colliders = Physics.OverlapBox(position, Vector3.one * 1.25f);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out ActorManager actor))
            {
                if (!IsHit(owner, actor)) return;

                ApplyEffect(owner, actor);
            }
        }
    }
}
