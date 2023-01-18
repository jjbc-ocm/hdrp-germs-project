using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideUI : ListViewUI<GuideItemUI, GuideUI>
{
    public List<GuideData> Data { get; set; }

    protected override void OnRefreshUI()
    {
        DeleteItems();

        RefreshItems(Data, (item, data) =>
        {
            item.Data = data;
        });
    }
}
