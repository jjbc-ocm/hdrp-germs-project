using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameNetworkManager : MonoBehaviour
{
    #region Serializable

    [SerializeField]
    private GameObject[] shipPrefabs;

    [SerializeField]
    private Transform[] spawnPoints;

    #endregion

    #region Private



    #endregion

    #region Unity

    void Start()
    {
        IntstantiatePlayer();
    }

    void Update()
    {
        
    }

    #endregion

    #region Private

    private void IntstantiatePlayer()
    {
        var team = PhotonNetwork.LocalPlayer.GetTeam();

        var shipIndex = PhotonNetwork.LocalPlayer.GetShipIndex();

        var prefabName = shipPrefabs[shipIndex].name;

        var spawnPoint = spawnPoints[team];

        PhotonNetwork.Instantiate(prefabName, spawnPoint.position, spawnPoint.rotation);
    }

    #endregion
}
