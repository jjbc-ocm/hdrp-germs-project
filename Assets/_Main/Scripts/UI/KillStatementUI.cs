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
        textWinner.text = Winner.GetName();

        textLoser.text = Loser.GetName();

        textWinner.color = colorTeams[Winner.GetTeam()];

        textLoser.color = colorTeams[Loser.GetTeam()];
    }

    private IEnumerator YieldClose()
    {
        yield return new WaitForSeconds(Constants.RESPAWN_TIME);

        Close();
    }
}
