using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using TMPro;
using UnityEngine;

public class ChatUI : ListViewUI<ChatItemUI, ChatUI>
{
    [SerializeField]
    private TMP_InputField inputMessage;

    public List<ChatData> Data { get; set; }

    public void OnSendMessage(string message)
    {
        var player = Player.Mine;

        var playerView = player.photonView;

        playerView.RPC("RpcSendChat", RpcTarget.All, playerView.GetName(), message, playerView.GetTeam(), DateTime.Now.Ticks);
    } 

    protected override void OnRefreshUI()
    {
        DeleteItems();

        RefreshItems(Data, (item, data) =>
        {
            item.Data = data;
        });
    }
}
