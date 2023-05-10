using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageBroadcastUI : UI<MessageBroadcastUI>
{
    [SerializeField]
    private TMP_Text textPlayer;

    [SerializeField]
    private TMP_Text textStatement;

    [SerializeField]
    private TMP_Text textTarget;

    [SerializeField]
    private Color[] colorTeams;

    public ActorManager Player { get; set; }

    public MessageBroadcastType Type { get; set; }

    private void OnEnable()
    {
        StopAllCoroutines();

        StartCoroutine(YieldClose());
    }

    protected override void OnRefreshUI()
    {
        textPlayer.text = Player.GetName();

        textStatement.text =
            Type == MessageBroadcastType.KeyObtained || Type == MessageBroadcastType.ChestObtained ? "obtained the" :
            Type == MessageBroadcastType.KeyDropped || Type == MessageBroadcastType.ChestDropped ? "dropped the" :
            "";

        textTarget.text =
            Type == MessageBroadcastType.ChestObtained || Type == MessageBroadcastType.ChestDropped ? "<color=yellow>Chest</color>" :
            Type == MessageBroadcastType.KeyObtained || Type == MessageBroadcastType.KeyDropped ? "<color=yellow>Key</color>" :
            "";

        textPlayer.color = colorTeams[Player.GetTeam()];
    }

    private IEnumerator YieldClose()
    {
        yield return new WaitForSeconds(5);

        Close();
    }
}
