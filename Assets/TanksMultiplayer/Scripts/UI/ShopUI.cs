using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : ListViewUI<ShopItemUI, ShopUI>
{
    [SerializeField]
    private Button buttonBuy;

    [Header("Selected Item Info")]

    [SerializeField]
    private GameObject uiSelectedInfo;

    [SerializeField]
    private TMP_Text textName;

    [SerializeField]
    private TMP_Text textDesc;

    [SerializeField]
    private Image imageSprite;

    public List<ItemData> Data { get; set; }

    public ItemData Selected { get; set; }

    protected override void OnRefreshUI()
    {
        var hasSelected = Selected != null;

        DeleteItems();

        RefreshItems(Data, (item, data) =>
        {
            item.Data = data;
        });

        buttonBuy.gameObject.SetActive(hasSelected);

        uiSelectedInfo.SetActive(hasSelected);

        if (hasSelected)
        {
            textName.text = Selected.Name;

            textDesc.text = Selected.Desc;

            imageSprite.sprite = Selected.Icon;
        }
    }

    public void OnBuyButtonClick()
    {
        // TODO: need to handle cost validation
        ShopManager.Instance.Buy(Selected);
    }
}
