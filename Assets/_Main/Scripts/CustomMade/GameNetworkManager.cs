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

    private bool hasBeenDisconnected;

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

    /*public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.ReconnectAndRejoin();
    }*/

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // must spawn a decoy ship on position where player got disconnected
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        hasBeenDisconnected = true;

        PhotonNetwork.Reconnect();
    }

    // It is called when reconnecting the game when the player got disconnected
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.RejoinRoom(Globals.ROOM_NAME);
    }

    // These methods are not called in MenuNetworkManager if player is not the master client, so they must be handled in these scene
    // NOTE: MenuNetworkManager must be destroyed when changing scenes to avoid conflict in logic
    public override void OnJoinedRoom()
    {
        if (!hasBeenDisconnected)
        {
            var playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

            PhotonNetwork.LocalPlayer.Initialize(playerCount % 2, GPCrewScreen.Instance.SelectedShip.m_prefabListIndex);
        }

        // TODO: need to handle if it is a first join or just a rejoin
    }

    // These methods are not called in MenuNetworkManager if player is not the master client, so they must be handled in these scene
    // NOTE: MenuNetworkManager must be destroyed when changing scenes to avoid conflict in logic
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

    private bool IsAllowedToReconnect(DisconnectCause cause)
    {
        return 
            cause == DisconnectCause.Exception ||
            cause == DisconnectCause.ServerTimeout ||
            cause == DisconnectCause.ClientTimeout ||
            cause == DisconnectCause.DisconnectByServerLogic ||
            cause == DisconnectCause.DisconnectByServerReasonUnknown;
    }

    #endregion
}
