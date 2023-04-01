using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreparationUI : WindowUI<PreparationUI>
{
    [SerializeField]
    private TMP_Text textTimer;

    [SerializeField]
    private PrepShipUI[] uiTeam1;

    [SerializeField]
    private PrepShipUI[] uiTeam2;

    [SerializeField]
    private SelectShipUI uiSelectShip;

    public bool IsUpdateTimer { get; set; }

    public bool IsUpdatePrepShip { get; set; }

    public bool IsUpdateSelectShip { get; set; }

    public void OnAnchorClick(GameObject button)
    {
        uiSelectShip.Close();

        button.SetActive(false);
    }

    protected override void OnRefreshUI()
    {
        if (IsUpdateTimer)
        {
            var time = PhotonNetwork.Time - PhotonNetwork.CurrentRoom.GetTimePrepSceneLoaded();

            textTimer.text = $"{Mathf.Max(0, Mathf.RoundToInt((float)(SOManager.Instance.Constants.PreparationTime - time)))}";
        }

        if (IsUpdatePrepShip)
        {
            var bots = PhotonNetwork.CurrentRoom.GetBots();

            var players = PhotonNetwork.CurrentRoom.Players.Select(i => i.Value);

            var team1Players = players.Where(i => i.GetTeam() == 0).ToArray();

            var team2Players = players.Where(i => i.GetTeam() == 1).ToArray();

            var team1Bots = bots.Where(i => i.Team == 0).ToList();

            var team2Bots = bots.Where(i => i.Team == 1).ToList();

            var team1FreeSlots = SOManager.Instance.Constants.MaxPlayerPerTeam - team1Players.Count();

            var team2FreeSlots = SOManager.Instance.Constants.MaxPlayerPerTeam - team2Players.Count();

            /* Update UI for players */
            for (int i = 0; i < uiTeam1.Length; i++)
            {
                var data = i < team1Players.Length ? team1Players[i] : null;

                uiTeam1[i].RefreshUI((self) =>
                {
                    self.Data = data;
                });
            }

            for (int i = 0; i < uiTeam2.Length; i++)
            {
                var data = i < team2Players.Length ? team2Players[i] : null;

                uiTeam2[i].RefreshUI((self) =>
                {
                    self.Data = data;
                });
            }

            /* Update UI for bots */
            for (var i = 0; i < team1FreeSlots; i++)
            {
                var bot = team1Bots[i];

                uiTeam1[i + team1Players.Length].RefreshUI((self) =>
                {
                    self.BotData = bot;
                });
            }

            for (var i = 0; i < team2FreeSlots; i++)
            {
                var bot = team2Bots[i];

                uiTeam2[i + team2Players.Length].RefreshUI((self) =>
                {
                    self.BotData = bot;
                });
            }
        }

        if (IsUpdateSelectShip)
        {
            uiSelectShip.RefreshUI();
        }

        IsUpdateTimer = false;

        IsUpdatePrepShip = false;

        IsUpdateSelectShip = false;
    }
}
