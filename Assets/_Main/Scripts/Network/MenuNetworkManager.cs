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

    private Action<string, float> onStatusChange;

    private double timeJoined;

    private double timeLastPlayerJoined;

    private bool isConnecting;

    private bool isMatchInitiated;

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

        if (!isMatchInitiated && lapseTime >= 15 && lapseTime < 999)
        {
            TryLoadPreparationScene(true);
        }
    }

    #endregion

    #region Photon

    public override void OnConnectedToMaster()
    {
        onStatusChange.Invoke("Attempting to join a room...", 0.2f);

        if (isConnecting)
        {
            StartMatchMaking();

            isConnecting = false;
        }
        
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        onStatusChange.Invoke("Error " + cause.ToString(), 0f);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        onStatusChange.Invoke("Create own room instead...", 0.3f);

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = SOManager.Instance.Constants.MaxPlayerCount });
    }

    public override void OnJoinedRoom()
    {
        timeJoined = PhotonNetwork.Time;

        TryLoadPreparationScene();
    }

    public override void OnLeftRoom()
    {
        onStatusChange.Invoke("Match making canceled...", 0f);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        timeLastPlayerJoined = PhotonNetwork.Time;

        TryLoadPreparationScene();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        TryLoadPreparationScene();
    }

    #endregion

    #region Public

    public void StartMatchMaking(Action<string, float> onStatusChange)
    {
        this.onStatusChange = onStatusChange;

        onStatusChange.Invoke("Start a match...", 0.1f);

        PhotonNetwork.NickName = PlayerPrefs.GetString(APIManager.Instance.PlayerData.Name);

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
        PhotonNetwork.LeaveRoom();
    }

    #endregion

    #region Private

    private void StartMatchMaking()
    {
        PhotonNetwork.JoinRandomRoom(
                new ExitGames.Client.Photon.Hashtable { /* Enter game mode here in the future */ },
                SOManager.Instance.Constants.MaxPlayerCount);
    }

    private void TryLoadPreparationScene(bool isIgnorePlayerCount = false)
    {
        var playerCountCondition = !isIgnorePlayerCount && PhotonNetwork.CurrentRoom.PlayerCount < SOManager.Instance.Constants.MaxPlayerCount;

        if (!PhotonNetwork.IsMasterClient || playerCountCondition) return;

        SetTeams();

        onStatusChange.Invoke("Loading scene...", 0.9f);

        PhotonNetwork.LoadLevel(SOManager.Instance.Constants.ScenePreparation);

        isMatchInitiated = true;
    }

    private void SetTeams()
    {
        var players = PhotonNetwork.PlayerList;

        var teamsCount = new int[2];

        var i = 0;

        do
        {
            var team = UnityEngine.Random.Range(0, 1);

            if (teamsCount[i] < SOManager.Instance.Constants.MaxPlayerPerTeam)
            {
                players[i].SetTeam(team);

                teamsCount[i] += 1;

                i += 1;
            }
        }
        while (i < players.Length);
    }

    #endregion
}
