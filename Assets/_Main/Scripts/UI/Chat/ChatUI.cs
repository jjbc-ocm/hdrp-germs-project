using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using TMPro;
using UnityEngine;

public class ChatUI : WindowListViewUI<ChatItemUI, ChatUI>
{
    [SerializeField]
    private TMP_InputField inputMessage;

    [SerializeField]
    private float minHeight;

    [SerializeField]
    private float maxHeight;

    public TMP_InputField InputMessage { get => inputMessage; }

    public List<ChatData> Data { get; set; }

    public bool IsMaximized { get; set; }

    private void Update()
    {
        var rectTransform = GetComponent<RectTransform>();

        var targetHeight = IsMaximized ? maxHeight : minHeight;

        var x = rectTransform.sizeDelta.x;

        var y = Mathf.Lerp(rectTransform.sizeDelta.y, targetHeight, Time.deltaTime * 10f);

        rectTransform.sizeDelta = new Vector2(x, y);
    }

    public void OnSendMessage(string message)
    {
        var player = PlayerManager.Mine;

        var playerView = player.photonView;

        playerView.RPC("RpcSendChat", RpcTarget.All, player.GetName(), message, player.GetTeam(), DateTime.Now.Ticks);
    } 

    protected override void OnRefreshUI()
    {
        DeleteItems();

        RefreshItems(Data, (item, data) =>
        {
            item.Data = data;
        });

        if (IsMaximized)
        {
            inputMessage.Select();

            inputMessage.ActivateInputField();
        }
    }
}
