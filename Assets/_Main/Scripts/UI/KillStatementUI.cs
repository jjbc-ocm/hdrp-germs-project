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

    public PhotonView Winner { get; set; }

    public PhotonView Loser { get; set; }

    void OnEnable()
    {
        StopAllCoroutines();

        StartCoroutine(YieldClose());
    }

    protected override void OnRefreshUI()
    {
        var isWinnerMonster = Winner.GetComponent<ActorManager>().IsMonster;

        var isLoserMonster = Loser.GetComponent<ActorManager>().IsMonster;

        textWinner.text = !isWinnerMonster ? Winner.GetName() : "Monster";

        textLoser.text = !isLoserMonster ? Loser.GetName() : "Monster";

        textWinner.color = colorTeams[!isWinnerMonster ? Winner.GetTeam() : 2];

        textLoser.color = colorTeams[!isLoserMonster ? Loser.GetTeam() : 2];
    }

    private IEnumerator YieldClose()
    {
        yield return new WaitForSeconds(Constants.RESPAWN_TIME);

        Close();
    }
}
