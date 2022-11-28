using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameNetworkManager : MonoBehaviourPunCallbacks
{
    public static GameNetworkManager Instance;

    #region Serializable

    [SerializeField]
    private GameObject[] shipPrefabs;

    [SerializeField]
    private Transform[] spawnPoints;

    public Transform[] SpawnPoints { get => spawnPoints; }

    #endregion

    #region Private



    #endregion

    #region Unity

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            IntstantiatePlayer();
        }
    }

    #endregion

    #region Photon

    /* These methods are not called in MenuNetworkManager if player is not the master client, so they must be handled in these scene */

    public override void OnJoinedRoom()
    {
        var playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        PhotonNetwork.LocalPlayer.Initialize(playerCount % 2, GPCrewScreen.m_selectedShip.m_prefabListIndex);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer == PhotonNetwork.LocalPlayer && 
            changedProps.ContainsKey(Constants.KEY_SHIP_INDEX) && 
            changedProps.ContainsKey(Constants.KEY_TEAM))
        {
            IntstantiatePlayer();
        }
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
