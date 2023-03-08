using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AftermathUI : UI<AftermathUI>
{
    /*[SerializeField]
    private TMP_Text textMessage;

    [SerializeField]
    private GameObject indicatorVictory;

    [SerializeField]
    private GameObject indicatorDefeat;

    [SerializeField]
    private GameObject indicatorDraw;

    [SerializeField]
    private GameObject indicatorLoad;

    public Team WinnerTeam { get; set; }

    public BattleResultType BattleResult { get; set; }

    public bool IsMessageDone { get; set; }

    void OnEnable()
    {
        StartCoroutine(YieldProceed());
    }

    protected override void OnRefreshUI()
    {
        textMessage.gameObject.SetActive(!IsMessageDone);

        if (!IsMessageDone)
        {
            textMessage.text = WinnerTeam != null
                ? "TEAM <color=#" + ColorUtility.ToHtmlStringRGB(WinnerTeam.material.color) + ">" + WinnerTeam.name + "</color> WINS!"
                : "DRAW!";
        }
        else
        {
            indicatorVictory.SetActive(BattleResult == BattleResultType.Victory);

            indicatorDefeat.SetActive(BattleResult == BattleResultType.Defeat);

            indicatorDraw.SetActive(BattleResult == BattleResultType.Draw);
        }
    }

    public async void OnClick()
    {
        indicatorLoad.SetActive(true);

        PhotonNetwork.Disconnect();

        var stats = APIManager.Instance.PlayerData.Stats;

        await APIManager.Instance.PlayerData
            .SetStats(new StatsData
            {
                Kills = stats.Kills + Player.Mine.Stat.Kills,

                Deaths = stats.Deaths + Player.Mine.Stat.Deaths,

                Wins = stats.Wins + (BattleResult == BattleResultType.Victory ? 1 : 0),

                Losses = stats.Losses + (BattleResult == BattleResultType.Defeat ? 1 : 0),

                Draws = stats.Draws + (BattleResult == BattleResultType.Draw ? 1 : 0)
            })
            .Put();

        indicatorLoad.SetActive(false);

        SceneManager.LoadScene(SOManager.Instance.Constants.SceneMenu);
    }



    private IEnumerator YieldProceed()
    {
        yield return new WaitForSeconds(3);

        RefreshUI((self) =>
        {
            self.IsMessageDone = true;
        });
    }*/

    [SerializeField]
    private TMP_Text[] textScores;

    [SerializeField]
    private TMP_Text[] textChests;

    [SerializeField]
    private PlayerStatusesUI[] teams;

    public List<List<Player>> Data { get; set; }

    protected override void OnRefreshUI()
    {
        var scores = PhotonNetwork.CurrentRoom.GetScore();

        var chests = PhotonNetwork.CurrentRoom.GetChests();

        for (var i = 0; i < teams.Length; i++)
        {
            textScores[i].text = scores[i].ToString();

            textChests[i].text = chests[i].ToString();

            teams[i].RefreshUI((self) =>
            {
                self.Data = Data[i];
            });
        }
    }
}
