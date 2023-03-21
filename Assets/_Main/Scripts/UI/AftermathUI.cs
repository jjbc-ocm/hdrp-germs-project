using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AftermathUI : WindowUI<AftermathUI>
{
    [SerializeField]
    private GameObject uiInGame;

    [SerializeField]
    private GameObject indicatorLoad;

    [Header("Battle Status")]

    [SerializeField]
    private GameObject indicatorVictory;

    [SerializeField]
    private GameObject indicatorDefeat;

    [SerializeField]
    private GameObject indicatorDraw;

    [Header("Score Board")]

    [SerializeField]
    private TMP_Text[] textScores;

    [SerializeField]
    private TMP_Text[] textChests;

    [SerializeField]
    private PlayerStatusesUI[] teams;

    [Header("Other Info")]

    [SerializeField]
    private GPShipCard uiShip;

    public List<List<Player>> Data { get; set; }

    public BattleResultType BattleResult { get; set; }

    public async void OnHomeButtonClick()
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

    protected override void OnRefreshUI()
    {
        uiInGame.SetActive(false);

        RefreshBattleStatus();

        RefreshScoreBoard();

        RefreshOtherInfo();
    }

    private void RefreshBattleStatus()
    {
        indicatorVictory.SetActive(BattleResult == BattleResultType.Victory);

        indicatorDefeat.SetActive(BattleResult == BattleResultType.Defeat);

        indicatorDraw.SetActive(BattleResult == BattleResultType.Draw);
    }
    private void RefreshScoreBoard()
    {
        for (var i = 0; i < teams.Length; i++)
        {
            textScores[i].text = PhotonNetwork.CurrentRoom.GetScore(i).ToString();

            textChests[i].text = PhotonNetwork.CurrentRoom.GetChest(i).ToString();

            teams[i].RefreshUI((self) =>
            {
                self.Data = Data[i];
            });
        }
    }

    private void RefreshOtherInfo()
    {
        uiShip.DisplayShipDesc(Player.Mine.Data);
    }
}
