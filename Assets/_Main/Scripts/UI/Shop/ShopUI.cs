using System.Collections;
using System.Collections.Generic;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : UI<ShopUI>
{
    [Header("Main Contents")]

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

    [Header("Recipe Item Info")]
    [SerializeField]
    private RecipeItemUI prefabRecipeItem;

    [SerializeField]
    private Transform[] transformRecipeLayers;

    public List<ItemData> Data { get; set; }

    public ItemData SelectedData { get; set; }

    public int SelectedSlotIndex { get; set; }

    void OnEnable()
    {
        OnRecoItemsClick();
    }

    protected override void OnRefreshUI()
    {
        var data = 
            SelectedData != null ? SelectedData :
            SelectedSlotIndex > -1 ? Player.Mine.Inventory.Items[SelectedSlotIndex] : 
            null;

        buttonBuy.gameObject.SetActive(data != null);

        buttonSell.gameObject.SetActive(SelectedSlotIndex > -1 && Player.Mine.Inventory.Items[SelectedSlotIndex] != null);

        uiSelectedInfo.SetActive(data != null);

        if (data != null)
        {
            var totalCost = ShopManager.Instance.GetTotalCost(data);

            buttonBuy.enabled = Player.Mine.Inventory.Gold >= totalCost;

            textName.text = data.Name;

            textDesc.text = data.Desc;

            textCost.text = totalCost.ToString();

            textBuyCost.text = totalCost.ToString();

            textSellCost.text = data.CostSell.ToString();

            imageSprite.sprite = data.Icon;

            foreach (var transform in transformRecipeLayers)
            {
                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }
            }

            CreateRecipeTree(data, 0);
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

            self.StatFilter = StatFilterType.All;
        });

        uiRecoItems.RefreshUI((self) =>
        {
            self.IsSelected = false;
        });

        uiRecoItems.Close();
    }

    public void OnBuyButtonClick()
    {
        var data =
            SelectedData != null ? SelectedData :
            SelectedSlotIndex > -1 ? Player.Mine.Inventory.Items[SelectedSlotIndex] :
            null;

        ShopManager.Instance.Buy(data);
    }

    public void OnSellButtonClick()
    {
        ShopManager.Instance.Sell(SelectedSlotIndex);
    }

    private void CreateRecipeTree(ItemData item, int recipeLayer)
    {
        var recipeItem = Instantiate(prefabRecipeItem, transformRecipeLayers[recipeLayer]);

        recipeItem.RefreshUI((self) =>
        {
            self.Data = item;
        });

        foreach (var recipe in item.Recipes)
        {
            CreateRecipeTree(recipe, recipeLayer + 1);
        }
    }
}
