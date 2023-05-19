using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemsUI : UI<GemsUI>
{
    [SerializeField]
    private GemItemUI[] items;

    public InventoryDef[] Data { get; set; }

    protected override void OnRefreshUI()
    {
        for (var i = 0; i < items.Length; i++)
        {
            items[i].RefreshUI((self) =>
            {
                self.Data = Data[i];
            });
        }
    }
}
