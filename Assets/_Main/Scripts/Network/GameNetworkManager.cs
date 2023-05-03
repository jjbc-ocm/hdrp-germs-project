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

        if (!PhotonNetwork.IsMasterClient) return;

        if (PhotonNetwork.CurrentRoom.GetGameMode() == GameMode.Tutorial) return;

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
   /* public override void OnConnectedToMaster()
    {
        PhotonNetwork.RejoinRoom(Globals.ROOM_NAME);
    }*/

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
        Debug.Log("Game Mode: " + PhotonNetwork.CurrentRoom.GetGameMode());

        var bots = PhotonNetwork.CurrentRoom.GetBots();

        var players = PhotonNetwork.CurrentRoom.Players.Select(i => i.Value);

        if (bots == null) return;

        var team1Players = players.Where(i => i.GetTeam() == 0).ToArray();

        var team2Players = players.Where(i => i.GetTeam() == 1).ToArray();

        var team1Bots = bots.Where(i => i.Team == 0).ToList();

        var team2Bots = bots.Where(i => i.Team == 1).ToList();

        var team1FreeSlots = SOManager.Instance.Constants.MaxPlayerPerTeam - team1Players.Count();

        var team2FreeSlots = SOManager.Instance.Constants.MaxPlayerPerTeam - team2Players.Count();

        for (var i = 0; i < team1FreeSlots; i++)
        {
            var bot = team1Bots[i];

            InstantiateBot(bot);
        }

        for (var i = 0; i < team2FreeSlots; i++)
        {
            var bot = team2Bots[i];

            InstantiateBot(bot);
        }
    }

    private void InstantiatePlayer(int team, int shipIndex)
    {
        var prefabName = SOManager.Instance.PlayerShips[shipIndex].m_playerPrefab.name;

        var spawnPosition = GameManager.Instance.GetBase(team).GetSpawnPosition();

        var spawnRotation = GameManager.Instance.GetBase(team).GetSpawnRotation();

        PhotonNetwork.Instantiate(prefabName, spawnPosition, spawnRotation);
    }

    private void InstantiateBot(BotInfo botInfo)
    {
        var prefabName = SOManager.Instance.BotShips[botInfo.ShipIndex].m_playerPrefab.name;

        var spawnPosition = GameManager.Instance.GetBase(botInfo.Team).GetSpawnPosition();

        var spawnRotation = GameManager.Instance.GetBase(botInfo.Team).GetSpawnRotation();

        var player = PhotonNetwork.InstantiateRoomObject(prefabName, spawnPosition, spawnRotation);

        player.GetComponent<PlayerManager>().Bot.Initialize(botInfo.BotIndex);
    }

    #endregion
}
