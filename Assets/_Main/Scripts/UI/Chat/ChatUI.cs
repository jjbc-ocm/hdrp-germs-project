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

    [SerializeField]
    private float minHeight;

    [SerializeField]
    private float maxHeight;

    private RectTransform rectTransform;

    public List<ChatData> Data { get; set; }

    public bool IsMaximized { get; set; }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        var targetHeight = IsMaximized ? maxHeight : minHeight;

        var x = rectTransform.sizeDelta.x;

        var y = Mathf.Lerp(rectTransform.sizeDelta.y, targetHeight, Time.deltaTime * 10f);

        rectTransform.sizeDelta = new Vector2(x, y);
    }

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
