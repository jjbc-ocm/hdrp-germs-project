using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : UI<ShopItemUI>
{
    [SerializeField]
    private Image imageSprite;

    [SerializeField]
    private GameObject selectedIndicator;

    public ItemData Data { get; set; }

    protected override void OnRefreshUI()
    {
        imageSprite.sprite = Data.Icon;

        selectedIndicator.SetActive(ShopManager.Instance.UI.Selected == Data);
    }

    public void OnClick()
    {
        ShopManager.Instance.UI.RefreshUI((ui) =>
        {
            ui.Selected = Data;
        });
    }
}
