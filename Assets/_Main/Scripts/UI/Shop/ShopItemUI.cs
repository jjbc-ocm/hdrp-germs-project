using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : UI<ShopItemUI>
{
    [SerializeField]
    private Image imageSprite;

    [SerializeField]
    private TMP_Text textName;

    [SerializeField]
    private TMP_Text textCost;

    [SerializeField]
    private ItemSO data;

    public ItemSO Data { get => data; set => data = value; }

    protected override void OnRefreshUI()
    {
        imageSprite.sprite = Data.Icon;

        textName.text = Data.Name;

        textCost.text = Data.CostBuy.ToString();
    }

    public void OnClick()
    {
        ShopManager.Instance.UI.RefreshUI((ui) =>
        {
            ui.SelectedData = Data;

            ui.SelectedSlotIndex = -1;
        });
    }
}
