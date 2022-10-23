using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ItemSpawnerManager : MonoBehaviourPun
{
    [System.Serializable]
    public class ItemSpawner
    {
        [SerializeField]
        private NavMeshAgent agent;

        [SerializeField]
        private GameObject prefab;

        [SerializeField]
        private float delay;

        [SerializeField]
        private int maxLimit;

        private float dt;

        public NavMeshAgent Agent { get => agent; }

        public GameObject Prefab { get => prefab; }

        public float Delay { get => delay; }

        public int MaxLimit { get => maxLimit; }

        public float Dt { get => dt; set => dt = value; }
    }

    [SerializeField]
    private ItemSpawner[] spawners;

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        for (var i = 0; i < spawners.Length; i++)
        {
            var spawner = spawners[i];

            if (spawner.Dt >= spawner.Delay)
            {
                spawner.Dt = 0;

                if (CanSpawn(spawner))
                {
                    RandomNavmeshLocation(i, 200, (position) =>
                    {
                        PhotonNetwork.InstantiateRoomObject(spawner.Prefab.name, position, Quaternion.identity);
                    });
                }
            }
            else
            {
                spawner.Dt += Time.deltaTime;
            }
        }
    }
    private bool CanSpawn(ItemSpawner spawner)
    {
        //return GameObject.FindGameObjectWithTag(obj.tag) != null;

        var objWithTags = GameObject.FindGameObjectsWithTag(spawner.Prefab.tag);

        return objWithTags.Length < spawner.MaxLimit;
    } 
    private void RandomNavmeshLocation(int spawnerIndex, float radius, Action<Vector3> onSetDestination)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;

        randomDirection.y = 0;

        randomDirection += transform.position;

        if (spawners[spawnerIndex].Agent.SetDestination(randomDirection))
        {
            StartCoroutine(YieldRandomNavmeshLocation(spawners[spawnerIndex].Agent, onSetDestination));
        }
    }

    /// <summary>
    /// Q: Why is it necessary than just to pass the position directly?
    /// A: When SetDestination() is called, it needs time to calculate the path and will not return anything in an instant.
    /// </summary>
    private IEnumerator YieldRandomNavmeshLocation(NavMeshAgent agent, Action<Vector3> onSetDestination)
    {
        yield return new WaitForSeconds(0.5f);

        onSetDestination.Invoke(agent.path.corners[agent.path.corners.Length - 1]);
    }
}
