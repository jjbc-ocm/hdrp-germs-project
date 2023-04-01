using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class PlayerStatusesUI : ListViewUI<PlayerStatusUI, PlayerStatusesUI>
{
    public List<PlayerManager> Data { get; set; }

    protected override void OnRefreshUI()
    {
        DeleteItems();

        RefreshItems(Data, (item, data) =>
        {
            item.Data = data;
        });
    }
}
