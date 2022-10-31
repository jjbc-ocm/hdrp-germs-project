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
        onStatusChange.Invoke("Attempting to join a room...");

        if (isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();

            isConnecting = false;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        onStatusChange.Invoke("Error " + cause.ToString());
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        onStatusChange.Invoke("Player created a room instead...");

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = Constants.MAX_PLAYER_COUNT });
    }

    public override void OnJoinedRoom()
    {
        onStatusChange.Invoke("Player joined a room...");

        var playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        PhotonNetwork.LocalPlayer.Initialize(playerCount % 2, playerCount - 1);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        onStatusChange.Invoke("Updating player info...");

        if (targetPlayer == PhotonNetwork.LocalPlayer &&
            changedProps.ContainsKey(Constants.KEY_SHIP_INDEX) &&
            changedProps.ContainsKey(Constants.KEY_TEAM))
        {
            TryLoadGame();
        }
    }

    #endregion

    #region Public

    public void Play(Action<string> onStatusChange)
    {
        this.onStatusChange = onStatusChange;

        onStatusChange.Invoke("Attempting to join a room...");

        PhotonNetwork.NickName = PlayerPrefs.GetString(TanksMP.PrefsKeys.playerName);

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            isConnecting = PhotonNetwork.ConnectUsingSettings();

            PhotonNetwork.GameVersion = Constants.NETWORK_VERSION;
        }
    }

    #endregion

    #region Private

    private void TryLoadGame()
    {
        onStatusChange.Invoke("Loading scene...");

        if (!PhotonNetwork.IsMasterClient) return;

        PhotonNetwork.LoadLevel(Constants.GAME_SCENE_NAME);
    }

    #endregion
}
