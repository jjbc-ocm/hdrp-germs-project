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

    #endregion

    #region Photon
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        var roomOptions = new RoomOptions
        {
            MaxPlayers = Constants.MAX_PLAYER_COUNT,

            PlayerTtl = int.MaxValue,

            EmptyRoomTtl = 10000
        };

        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        var playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        PhotonNetwork.LocalPlayer.Initialize(playerCount % 2, GPCrewScreen.Instance.SelectedShip.m_prefabListIndex);
    }
    #endregion

    #region Public

    public void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    #endregion

}
