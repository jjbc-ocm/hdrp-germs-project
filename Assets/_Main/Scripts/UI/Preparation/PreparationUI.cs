using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PreparationUI : WindowUI<PreparationUI>
{
    [SerializeField]
    private PrepShipUI[] uiTeam1;

    [SerializeField]
    private PrepShipUI[] uiTeam2;

    protected override void OnRefreshUI()
    {
        var players = PhotonNetwork.PlayerList;

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
}
