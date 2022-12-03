using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllItemsUI : ListViewUI<ShopItemUI, AllItemsUI>
{
    [SerializeField]
    private GameObject buttonSelectedIndicator;

    public bool IsSelected { get; set; }

    protected override void OnRefreshUI()
    {
        buttonSelectedIndicator.SetActive(IsSelected);

        DeleteItems();

        RefreshItems(ShopManager.Instance.Data, (item, data) =>
        {
            item.Data = data;
        });
    }
}
