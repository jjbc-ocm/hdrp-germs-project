using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using TMPro;
using UnityEngine;

public class KillStatementUI : UI<KillStatementUI>
{
    [SerializeField]
    private TMP_Text textWinner;

    [SerializeField]
    private TMP_Text textLoser;

    [SerializeField]
    private Color[] colorTeams;

    public ActorManager Winner { get; set; }

    public ActorManager Loser { get; set; }

    void OnEnable()
    {
        StopAllCoroutines();

        StartCoroutine(YieldClose());
    }

    protected override void OnRefreshUI()
    {
        var isWinnerMonster = Winner.IsMonster;

        var isLoserMonster = Loser.IsMonster;

        textWinner.text = !isWinnerMonster ? Winner.GetName() : "Monster";

        textLoser.text = !isLoserMonster ? Loser.GetName() : "Monster";

        textWinner.color = colorTeams[!isWinnerMonster ? Winner.GetTeam() : 2];

        textLoser.color = colorTeams[!isLoserMonster ? Loser.GetTeam() : 2];
    }

    private IEnumerator YieldClose()
    {
        yield return new WaitForSeconds(SOManager.Instance.Constants.RespawnTime);

        Close();
    }
}
