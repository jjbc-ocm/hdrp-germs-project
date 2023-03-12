using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MatchMakingUI : WindowUI<MatchMakingUI>
{
    [SerializeField]
    private TMP_Text textTime;

    [SerializeField]
    private TMP_Text textPlayers;

    private void Update()
    {
        RefreshUI();
    }

    public void OnCancelClick()
    {
        MenuNetworkManager.Instance.CancelMatchMaking();

        Close();
    }

    protected override void OnRefreshUI()
    {
        if (PhotonNetwork.CurrentRoom == null)
        {
            textTime.text = "---";

            textPlayers.text = "Joining game...";
        }
        else
        {
            var time = TimeSpan.FromSeconds(PhotonNetwork.Time - MenuNetworkManager.Instance.TimeJoined);

            textTime.text = string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);

            textPlayers.text = string.Format("{0}/{1}", PhotonNetwork.CurrentRoom.PlayerCount, SOManager.Instance.Constants.MaxPlayerCount);
        }
    }
}
