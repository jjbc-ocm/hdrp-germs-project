using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : UI<ShopUI>//ListViewUI<ShopItemUI, ShopUI>
{
    [SerializeField]
    private RecoItemsUI uiRecoItems;

    [SerializeField]
    private AllItemsUI uiAllItems;

    [SerializeField]
    private Button buttonBuy;

    [SerializeField]
    private Button buttonSell;

    [SerializeField]
    private TMP_Text textBuyCost;

    [SerializeField]
    private TMP_Text textSellCost;

    [Header("Selected Item Info")]

    [SerializeField]
    private GameObject uiSelectedInfo;

    [SerializeField]
    private TMP_Text textName;

    [SerializeField]
    private TMP_Text textDesc;

    [SerializeField]
    private TMP_Text textCost;

    [SerializeField]
    private Image imageSprite;

    public List<ItemData> Data { get; set; }

    public ItemData Selected { get; set; }

    void OnEnable()
    {
        OnRecoItemsClick();
    }

    protected override void OnRefreshUI()
    {
        var hasSelected = Selected != null;

        /*DeleteItems();

        RefreshItems(Data, (item, data) =>
        {
            item.Data = data;
        });*/

        buttonBuy.gameObject.SetActive(hasSelected);

        uiSelectedInfo.SetActive(hasSelected);

        if (hasSelected)
        {
            textName.text = Selected.Name;

            textDesc.text = Selected.Desc;

            textBuyCost.text = Selected.CostBuy.ToString();

            textSellCost.text = Selected.CostSell.ToString();

            imageSprite.sprite = Selected.Icon;
        }
    }

    public void OnRecoItemsClick()
    {
        uiRecoItems.Open((self) =>
        {
            self.IsSelected = true;
        });

        uiAllItems.RefreshUI((self) =>
        {
            self.IsSelected = false;
        });

        uiAllItems.Close();
    }

    public void OnAllItemsClick()
    {
        uiAllItems.Open((self) =>
        {
            self.IsSelected = true;
        });

        uiRecoItems.RefreshUI((self) =>
        {
            self.IsSelected = false;
        });

        uiRecoItems.Close();
    }

    public void OnBuyButtonClick()
    {
        // TODO: need to handle cost validation
        ShopManager.Instance.Buy(Selected);
    }

    public void OnSellButtonClick()
    {

    }
}
