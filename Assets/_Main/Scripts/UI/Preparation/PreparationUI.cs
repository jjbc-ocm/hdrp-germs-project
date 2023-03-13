using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

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

    protected override void OnRefreshUI()
    {
        if (IsUpdateTimer)
        {
            var time = PhotonNetwork.Time - PhotonNetwork.CurrentRoom.GetTimePrepSceneLoaded();

            textTimer.text = $"{Mathf.RoundToInt((float)(SOManager.Instance.Constants.PreparationTime - time))}";
        }

        if (IsUpdatePrepShip)
        {
            var players = PhotonNetwork.CurrentRoom.Players.Select(i => i.Value);

            var team1 = players.Where(i => i.GetTeam() == 0).ToArray();

            var team2 = players.Where(i => i.GetTeam() == 1).ToArray();

            for (int i = 0; i < uiTeam1.Length; i++)
            {
                var data = i < team1.Length ? team1[i] : null;

                uiTeam1[i].RefreshUI((self) =>
                {
                    self.Data = data;
                });
            }

            for (int i = 0; i < uiTeam2.Length; i++)
            {
                var data = i < team2.Length ? team2[i] : null;

                uiTeam2[i].RefreshUI((self) =>
                {
                    self.Data = data;
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
