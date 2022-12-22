using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class ScoreBoardUI : WindowUI<ScoreBoardUI>
{
    [SerializeField]
    private PlayerStatusesUI [] teams;

    public List<List<Player>> Data { get; set; }

    protected override void OnRefreshUI()
    {
        for (var i = 0; i < teams.Length; i++)
        {
            teams[i].RefreshUI((self) =>
            {
                self.Data = Data[i];
            });
        }
    }
}
