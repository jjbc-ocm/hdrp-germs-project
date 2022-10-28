using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNetworkManager : MonoBehaviourPunCallbacks
{
    #region Serializables

    [SerializeField]
    private byte maxPlayers = 6;

    #endregion


    #region Private Variables

    private bool isConnecting;

    #endregion


    #region Unity

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    #endregion

    #region Photon

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");

        if (isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();

            isConnecting = false;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnDisconnected");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed");

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayers });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");

        var playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            { "team", playerCount % 2 },
            { "shipIndex", playerCount - 1 }
        });
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log("OnPlayerPropertiesUpdate");

        if (targetPlayer == PhotonNetwork.LocalPlayer)
        {
            TryLoadGame();
        }
    }

    #endregion

    #region Public

    public void Play()
    {
        Debug.Log("Play");

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
        Debug.Log("TryLoadGame");

        if (!PhotonNetwork.IsMasterClient) return;

        PhotonNetwork.LoadLevel(Constants.GAME_SCENE_NAME);
    }

    #endregion
}
