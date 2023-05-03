using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopBarUI : UI<TopBarUI>
{
    [System.Serializable]
    public struct TeamInfo
    {
        [SerializeField]
        private MiniPlayerInfoUI[] uiPlayerInfo;

        [SerializeField]
        private Slider sliderChests;

        public MiniPlayerInfoUI[] UIPlayerInfo { get => uiPlayerInfo; }

        public Slider SliderChests { get => sliderChests; }
    }

    [SerializeField]
    private TeamInfo[] teams;

    [SerializeField]
    private TMP_Text textKills;

    [SerializeField]
    private TMP_Text textDeaths;

    void Update()
    {
        RefreshUI();
    }

    protected override void OnRefreshUI()
    {
        var team1 = GameManager.Instance.Team1Ships;

        var team2 = GameManager.Instance.Team2Ships;

        for (int team = 0; team < SOManager.Instance.Constants.MaxTeam; team++)
        {
            for (int i = 0; i < SOManager.Instance.Constants.MaxPlayerPerTeam; i++)
            {
                var players = team == 0 ? team1 : team2;

                var player = i < players.Count() ? players[i] : null;

                teams[team].UIPlayerInfo[i].gameObject.SetActive(player);

                if (player)
                {
                    teams[team].UIPlayerInfo[i].RefreshUI((self) =>
                    {
                        self.Data = player;
                    });
                }
            }

            teams[team].SliderChests.value = SOManager.Instance.Constants.MaxChestPerTeam - PhotonNetwork.CurrentRoom.GetChestLost(team);
        }

        if (PlayerManager.Mine == null) return;

        textKills.text = PlayerManager.Mine.Stat.Kills.ToString();

        textDeaths.text = PlayerManager.Mine.Stat.Deaths.ToString();
    }
}
