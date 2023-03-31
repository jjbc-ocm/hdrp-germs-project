using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProfileUI : WindowListViewUI<SettingCategoryUI, ProfileUI>
{
    [SerializeField]
    private TMP_Text textName;

    [SerializeField]
    private TMP_Text textWins;

    [SerializeField]
    private TMP_Text textLosses;

    [SerializeField]
    private TMP_Text textDraws;

    [SerializeField]
    private TMP_Text textKillDeath;

    [SerializeField]
    private TMP_Text textWinRate;

    public PlayerData Data { get; set; }

    protected override void OnRefreshUI()
    {
        var stats = Data.Stats;

        var killsDeaths = stats.Kills + stats.Deaths;

        var winsLosses = stats.Wins + stats.Losses;

        int kdRatio = killsDeaths > 0 ? Mathf.RoundToInt(stats.Kills / (float)killsDeaths * 100.0f) : 0;

        int winRate = winsLosses > 0 ? Mathf.RoundToInt(stats.Wins / winsLosses * 100.0f) : 0;

        textName.text = Data.Name;

        textWins.text = stats.Wins.ToString();

        textLosses.text = stats.Losses.ToString();

        textDraws.text = stats.Draws.ToString();

        textKillDeath.text = string.Format("{0}%", kdRatio);

        textWinRate.text = string.Format("{0}%", winRate);
    }
}
