using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNetworkManager : MonoBehaviourPunCallbacks
{
    public static MenuNetworkManager Instance;

    #region Serializables

    #endregion

    #region Private Variables

    private Action<string> onStatusChange;

    private double timeJoined;

    private double timeLastPlayerJoined;

    private bool isConnecting;

    private bool isPreparationInitiated;

    #endregion

    #region Accessor

    public double TimeJoined { get => timeJoined; }

    public double TimeLastPlayerJoined { get => timeLastPlayerJoined; }

    #endregion


    #region Unity

    private void Awake()
    {
        Instance = this;

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Update()
    {
        var lapseTime = PhotonNetwork.Time - timeLastPlayerJoined;

        if (!isPreparationInitiated && lapseTime >= 15)
        {
            TryInitializePlayers(true);
        }
    }

    #endregion

    #region Photon

    public override void OnConnectedToMaster()
    {
        onStatusChange.Invoke("Ready for match making...");

        if (isConnecting)
        {
            StartMatchMaking();

            isConnecting = false;
        }
        
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        onStatusChange.Invoke("Disconnect Error " + cause.ToString());
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        onStatusChange.Invoke("Cannot join room, creating one instead...");

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = SOManager.Instance.Constants.MaxPlayerCount });
    }

    public override void OnJoinedRoom()
    {
        onStatusChange.Invoke("Successfully joined a room...");

        timeJoined = PhotonNetwork.Time;

        timeLastPlayerJoined = PhotonNetwork.Time;

        PhotonNetwork.LocalPlayer.Initialize(GPCrewScreen.Instance.SelectedShip.m_prefabListIndex);

        //TryInitializePlayers();
    }

    public override void OnLeftRoom()
    {
        onStatusChange.Invoke("Match making canceled...");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        onStatusChange.Invoke(newPlayer.GetName() + " joined...");

        timeLastPlayerJoined = PhotonNetwork.Time;

        TryInitializePlayers();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        onStatusChange.Invoke(otherPlayer.GetName() + " left...");

        //TryInitializePlayers();
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (PhotonNetwork.CurrentRoom.IsTeamSetup())
        {
            onStatusChange.Invoke("Player initialization complete...");

            TryLoadPreparationScene();
        }
    }

    #endregion

    #region Public

    public void StartMatchMaking(Action<string> onStatusChange)
    {
        this.onStatusChange = onStatusChange;

        onStatusChange.Invoke("Starting a game...");

        if (PhotonNetwork.IsConnected)
        {
            StartMatchMaking();
        }
        else
        {
            isConnecting = PhotonNetwork.ConnectUsingSettings();

            PhotonNetwork.GameVersion = SOManager.Instance.Constants.NetworkVersion;
        }
    }

    public void CancelMatchMaking()
    {
        onStatusChange.Invoke("Match making canceled...");

        PhotonNetwork.LeaveRoom();
    }

    #endregion

    #region Private

    private void StartMatchMaking()
    {
        onStatusChange.Invoke("Joining random room...");

        PhotonNetwork.JoinRandomRoom(
                new ExitGames.Client.Photon.Hashtable { /* Enter game mode here in the future */ },
                SOManager.Instance.Constants.MaxPlayerCount);
    }

    private void TryInitializePlayers(bool isIgnorePlayerCount = false)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        var isMaxPlayers = PhotonNetwork.CurrentRoom.PlayerCount == SOManager.Instance.Constants.MaxPlayerCount;

        if (isMaxPlayers || isIgnorePlayerCount)
        {
            InitializePlayers();
        }
    }

    private void TryLoadPreparationScene()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        onStatusChange.Invoke("Loading preparation phase...");

        PhotonNetwork.LoadLevel(SOManager.Instance.Constants.ScenePreparation);
    }

    private void InitializePlayers()
    {
        onStatusChange.Invoke("Initializing players...");

        // TODO: somewhere here, there's an index out of range

        PhotonNetwork.CurrentRoom.IsOpen = false;

        PhotonNetwork.CurrentRoom.IsVisible = false;

        var players = PhotonNetwork.PlayerList;

        var teamsCount = new int[2];

        var i = 0;

        do
        {
            var team = UnityEngine.Random.Range(0, 2);

            if (teamsCount[team] < SOManager.Instance.Constants.MaxPlayerPerTeam)
            {
                players[i].SetTeam(team);

                teamsCount[team] += 1;

                i += 1;
            }
        }
        while (i < players.Length);

        PhotonNetwork.CurrentRoom.Initialize(true);

        isPreparationInitiated = true;
    }

    #endregion
}
