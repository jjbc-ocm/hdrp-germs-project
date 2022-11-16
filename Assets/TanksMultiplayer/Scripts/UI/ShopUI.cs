using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : ListViewUI<ShopItemUI, ShopUI>
{
    public List<ItemData> Data { get; set; }

    protected override void OnRefreshUI()
    {
        DeleteItems();

        RefreshItems(Data, (item, data) =>
        {
            item.Data = data;
        });
    }
}
