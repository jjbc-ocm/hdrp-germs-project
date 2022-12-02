using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TanksMP;
using UnityEngine;
using UnityEngine.AI;

public class ItemSpawnerManager : MonoBehaviourPun
{
    public static ItemSpawnerManager Instance;

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
        private bool isChest;

        [SerializeField]
        private int maxLimit;

        private float dt;

        public NavMeshAgent Agent { get => agent; }

        public GameObject Prefab { get => prefab; }

        public float Delay { get => delay; }

        public bool IsChest { get => isChest; }

        public int MaxLimit { get => maxLimit; }

        public float Dt { get => dt; set => dt = value; }
    }

    [SerializeField]
    private ItemSpawner[] spawners;

    public ItemSpawner[] Spawners { get => spawners; }

    void Awake()
    {
        Instance = this;
    }

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
        var objWithTags = GameObject.FindGameObjectsWithTag(spawner.Prefab.tag);

        if (spawner.IsChest)
        {
            var players = FindObjectsOfType<Player>();

            var hasChest = players.FirstOrDefault(i => i.photonView.HasChest()) != null;

            return objWithTags.Length < spawner.MaxLimit && !hasChest;
        }

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
