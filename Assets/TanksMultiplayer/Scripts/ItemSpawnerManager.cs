using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnerManager : MonoBehaviourPun
{
    [System.Serializable]
    public class ItemSpawner
    {
        [SerializeField]
        private GameObject item;

        [SerializeField]
        private float delay;

        private float dt;

        public GameObject Item { get => item; }

        public float Delay { get => delay; }

        public float Dt { get => dt; set => dt = value; }
    }

    [SerializeField]
    private ItemSpawner[] spawners;

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        foreach (var spawner in spawners)
        {
            spawner.Dt += Time.deltaTime;

            if (spawner.Dt >= spawner.Delay)
            {
                spawner.Dt -= spawner.Delay;

                photonView.RPC("SpawnItem", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    public void SpawnItem()
    {
        Debug.Log("Spawn Item called");
    }
}
