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

    private bool isConnecting;

    #endregion


    #region Unity

    void Awake()
    {
        Instance = this;

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    #endregion

    #region Photon

    public override void OnConnectedToMaster()
    {
        onStatusChange.Invoke("Attempting to join a room...", 0.2f);
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        /*
        if (isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();

            isConnecting = false;
        }
        */
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        TryLoadGame();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        onStatusChange.Invoke("Error " + cause.ToString(), 0f);
    }

    /*
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        onStatusChange.Invoke("Player will create a room instead...", 0.4f);

        var roomOptions = new RoomOptions
        {
            MaxPlayers = Constants.MAX_PLAYER_COUNT,

            PlayerTtl = int.MaxValue,

            EmptyRoomTtl = 10000
        };

        PhotonNetwork.CreateRoom(null, roomOptions);
    }
    */

    /*
    public override void OnJoinedRoom()
    {
        onStatusChange.Invoke("Player joined a room...", 0.6f);

        var playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        //PhotonNetwork.LocalPlayer.Initialize(playerCount % 2, GPCrewScreen.Instance.SelectedShip.m_prefabListIndex);
        PhotonNetwork.LocalPlayer.Initialize(-1, GPCrewScreen.Instance.SelectedShip.m_prefabListIndex);
    }
    */

    /*
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        onStatusChange.Invoke("Updating player info...", 0.8f);

        if (targetPlayer == PhotonNetwork.LocalPlayer &&
            changedProps.ContainsKey(Constants.KEY_SHIP_INDEX) &&
            changedProps.ContainsKey(Constants.KEY_TEAM))
        {
            TryLoadGame();
        }
    }
    */

    #endregion

    #region Public

    public void Play(Action<string, float> onStatusChange)
    {
        this.onStatusChange = onStatusChange;

        onStatusChange.Invoke("Attempting to join a room...", 0.2f);

        PhotonNetwork.NickName = PlayerPrefs.GetString(TanksMP.PrefsKeys.playerName);

        if (PhotonNetwork.IsConnected)
        {
            //PhotonNetwork.JoinRandomRoom();
            TryLoadGame();
        }
        else
        {
            isConnecting = PhotonNetwork.ConnectUsingSettings();

            PhotonNetwork.GameVersion = SOManager.Instance.Constants.NetworkVersion;
        }
    }

    #endregion

    #region Private

    private void TryLoadGame()
    {
        onStatusChange.Invoke("Loading scene...", 0.9f);

        //if (!PhotonNetwork.IsMasterClient) return;

        PhotonNetwork.LoadLevel(Constants.WAITING_ROOM_SCENE_NAME);
    }

    #endregion
}
