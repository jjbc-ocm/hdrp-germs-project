using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class GPWaitingRoomNetworkManager : MonoBehaviourPunCallbacks
{
    public static GPWaitingRoomNetworkManager Instance;

    #region Unity

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    #endregion

    #region Photon
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        /*
        var roomOptions = new RoomOptions
        {
            MaxPlayers = Constants.MAX_PLAYER_COUNT,

            PlayerTtl = int.MaxValue,

            EmptyRoomTtl = 10000
        };
        */

        // We failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable() {{ "WaitingForPlayers", true }};
        string[] lobbyProperties = { "WaitingForPlayers" };
        RoomOptions opt = new RoomOptions();
        opt.MaxPlayers = (byte)SOManager.Instance.Constants.MaxPlayerCount;
        opt.IsOpen = true;
        opt.IsVisible = true;
        opt.PublishUserId = true;
        opt.CustomRoomPropertiesForLobby = lobbyProperties;
        opt.CustomRoomProperties = roomProperties;
        opt.BroadcastPropsChangeToAll = true;
        opt.PlayerTtl = int.MaxValue;
        opt.EmptyRoomTtl = 10000;
        PhotonNetwork.CreateRoom(null, opt, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        var playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        PhotonNetwork.LocalPlayer.Initialize(GPCrewScreen.Instance.SelectedShip.m_prefabListIndex);
    }
    #endregion

    #region Public

    public void JoinRoom()
    {
        DoMatchMaking();
    }

    /// <summary>
    /// Try to join a room of the specified room type and match type
    /// </summary>
    public void DoMatchMaking()
    {
        // Join random room that meets the battle mode configurations
        ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable() { { "WaitingForPlayers", true } };
        PhotonNetwork.JoinRandomRoom(roomProperties, SOManager.Instance.Constants.MaxPlayerCount);
    }

    #endregion

}
