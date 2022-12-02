using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugNetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject[] shipPrefabs;

    [SerializeField]
    private Vector3 spawnPosition;

    private GameObject player;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            InstantiatePlayer(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            InstantiatePlayer(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            InstantiatePlayer(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            InstantiatePlayer(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            InstantiatePlayer(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            InstantiatePlayer(5);
        }
    }

    private void InstantiatePlayer(int index)
    {
        if (player != null)
        {
            PhotonNetwork.Destroy(player);
        }

        var prefabName = shipPrefabs[index].name;

        player = PhotonNetwork.Instantiate(prefabName, spawnPosition, Quaternion.identity);
    }
}
