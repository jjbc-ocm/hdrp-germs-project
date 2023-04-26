using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreparationNetworkManager : MonoBehaviourPunCallbacks
{
    #region Instance Variables

    private bool isMatchInitiated;

    #endregion

    #region Unity

    private void Start()
    {
        PreparationUI.Instance.RefreshUI((self) =>
        {
            self.IsUpdatePrepShip = true;

            self.IsUpdateSelectShip = true;
        });

        PhotonNetwork.CurrentRoom.SetTimePrepSceneLoaded(PhotonNetwork.Time);
    }

    private void Update()
    {
        PreparationUI.Instance.RefreshUI((self) =>
        {
            self.IsUpdateTimer = true;
        });

        var time = PhotonNetwork.Time - PhotonNetwork.CurrentRoom.GetTimePrepSceneLoaded();

        if (!isMatchInitiated && time >= SOManager.Instance.Constants.PreparationTime && time < 999)
        {
            TryLoadGameScene();

            isMatchInitiated = true;
        }
    }

    #endregion

    #region Private

    private void TryLoadGameScene()
    {
        LoadingUI.Instance.Open((self) =>
        {
            self.Text = "Loading scene...";

            self.Progress = 0.5f;
        });

        if (!PhotonNetwork.IsMasterClient) return;

        PhotonNetwork.LoadLevel(SOManager.Instance.Constants.SceneGame);
    }

    #endregion

    #region Photon

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        PreparationUI.Instance.RefreshUI((self) =>
        {
            self.IsUpdatePrepShip = true;
        });
    }

    #endregion
}
