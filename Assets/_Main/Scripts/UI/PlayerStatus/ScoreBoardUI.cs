using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using TMPro;
using UnityEngine;

public class ScoreBoardUI : WindowUI<ScoreBoardUI>
{
    [SerializeField]
    private TMP_Text[] textScores;

    [SerializeField]
    private TMP_Text[] textChests;

    [SerializeField]
    private PlayerStatusesUI [] teams;

    public List<List<PlayerManager>> Data { get; set; }

    protected override void OnRefreshUI()
    {
        //var scores = PhotonNetwork.CurrentRoom.GetScore();

        //var chests = PhotonNetwork.CurrentRoom.GetChests();

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
}
