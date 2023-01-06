using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatItemUI : UI<ChatItemUI>
{
    [SerializeField]
    private TMP_Text textSender;

    [SerializeField]
    private TMP_Text textMessage;

    [SerializeField]
    private Color[] colorTeams;

    public ChatData Data { get; set; }

    protected override void OnRefreshUI()
    {
        textSender.text = Data.Sender;

        textMessage.text = Data.Message;

        textSender.color = colorTeams[Data.Team];
    }
}
