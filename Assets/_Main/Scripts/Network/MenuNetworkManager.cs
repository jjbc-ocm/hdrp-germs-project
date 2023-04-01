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

    private string status;

    private double timeJoined;

    private double timeLastPlayerJoined;

    private bool isConnecting;

    private bool isPreparationInitiated;

    #endregion

    #region Accessor

    public string Status
    {
        get
        {
            var arg0 = Mathf.RoundToInt(Mathf.Max(0, (float)(15f - (PhotonNetwork.Time - timeLastPlayerJoined))));

            return string.Format(status, arg0);
        }
    }

    public double ElapsedTimeJoined { get => PhotonNetwork.Time - timeJoined; }

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
        status = "Ready for match making...";

        if (isConnecting)
        {
            StartMatchMaking();

            isConnecting = false;
        }
        
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        status = "Disconnect Error " + cause.ToString();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        status = "Cannot join room, creating one instead...";

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = SOManager.Instance.Constants.MaxPlayerCount });
    }

    public override void OnJoinedRoom()
    {
        status = "Waiting for other players in {0}.";

        timeJoined = PhotonNetwork.Time;

        timeLastPlayerJoined = PhotonNetwork.Time;

        PhotonNetwork.LocalPlayer.Initialize(GPCrewScreen.Instance.SelectedShip.m_prefabListIndex);
    }

    public override void OnLeftRoom()
    {
        status = "Match making canceled...";
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        status = "A player joined... Waiting for other players in {0}.";

        timeLastPlayerJoined = PhotonNetwork.Time;

        TryInitializePlayers();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        status = "A player left... Waiting for other players in {0}.";
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (PhotonNetwork.CurrentRoom.IsTeamSetup())
        {
            status = "Player initialization complete...";

            TryLoadPreparationScene();
        }
    }

    #endregion

    #region Public

    public void TryStartMatchMaking()
    {
        status = "Starting a game...";

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
        status = "Match making canceled...";

        PhotonNetwork.LeaveRoom();
    }

    #endregion

    #region Private

    private void StartMatchMaking()
    {
        status = "Joining random room...";

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

        status = "Loading preparation phase...";

        PhotonNetwork.LoadLevel(SOManager.Instance.Constants.ScenePreparation);
    }

    private void InitializePlayers()
    {
        status = "Initializing players...";

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
