using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GemItemUI : UI<GemItemUI>
{
    [SerializeField]
    private TMP_Text textName;

    [SerializeField]
    private TMP_Text textCost;

    public InventoryDef Data { get; set; }

    public void OnBuyClick()
    {
        IAPManager.Instance.BuyGem(Data);
    }

    protected override void OnRefreshUI()
    {
        textName.text = Data.Name;

        textCost.text = Data.LocalPriceFormatted;
    }
}
