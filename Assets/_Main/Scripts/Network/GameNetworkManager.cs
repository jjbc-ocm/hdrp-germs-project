using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TanksMP;
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
        var myPlayer = PhotonNetwork.LocalPlayer;

        InstantiatePlayer(myPlayer.GetTeam(), myPlayer.GetShipIndex());

        InstantiateBots();
    }

    #endregion

    #region Photon

    /*public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            InstantiateDecoyPlayer(otherPlayer.UserId);
        }
    }*/

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

    // TODO: removed because never called
    // These methods are not called in MenuNetworkManager if player is not the master client, so they must be handled in these scene
    // NOTE: MenuNetworkManager must be destroyed when changing scenes to avoid conflict in logic
    /*public override void OnJoinedRoom()
    {
        Debug.LogError("OnJoinedRoom");

        // Only initialize if the player has initially joined the game
        // It will not be executed when player got disconnected, then reconnected
        if (!hasBeenDisconnected)
        {
            var playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

            PhotonNetwork.LocalPlayer.Initialize(playerCount % 2, GPCrewScreen.Instance.SelectedShip.m_prefabListIndex);
        }
    }

    // These methods are not called in MenuNetworkManager if player is not the master client, so they must be handled in these scene
    // NOTE: MenuNetworkManager must be destroyed when changing scenes to avoid conflict in logic
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.LogError("OnPlayerPropertiesUpdate A");

        if (targetPlayer == PhotonNetwork.LocalPlayer && 
            changedProps.ContainsKey(Constants.KEY_SHIP_INDEX) && 
            changedProps.ContainsKey(Constants.KEY_TEAM))
        {
            Debug.LogError("OnPlayerPropertiesUpdate B");

            IntstantiatePlayer();
        }
    }*/

    // This method is only called when player rejoined the room
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {

    }

    #endregion

    #region Private

    private void InstantiateBots()
    {
        var bots = PhotonNetwork.CurrentRoom.GetBots();

        if (bots == null) return;

        foreach (var bot in bots)
        {
            InstantiatePlayer(bot.Team, bot.ShipIndex, bot);
        }
    }

    private void InstantiatePlayer(int team, int shipIndex, BotInfo botInfo = null)
    {
        var prefabName = shipPrefabs[shipIndex].name;

        var spawnPoint = spawnPoints[team];

        GameObject player;

        if (botInfo != null)
        {
            player = PhotonNetwork.InstantiateRoomObject(prefabName, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            player = PhotonNetwork.Instantiate(prefabName, spawnPoint.position, spawnPoint.rotation);
        }

        player.GetComponent<TanksMP.Player>().Initialize(botInfo);
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
