using System.Collections;
using System.Collections.Generic;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : UI<ShopUI>
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

    [Header("Recipe Item Info")]
    [SerializeField]
    private RecipeItemUI prefabRecipeItem;

    [SerializeField]
    private Transform[] transformRecipeLayers;

    public List<ItemData> Data { get; set; }

    public ItemData Selected { get; set; }

    void OnEnable()
    {
        OnRecoItemsClick();
    }

    protected override void OnRefreshUI()
    {
        var hasSelected = Selected != null;

        buttonBuy.gameObject.SetActive(hasSelected);

        buttonSell.gameObject.SetActive(Player.Mine.Inventory.HasItem(Selected));

        uiSelectedInfo.SetActive(hasSelected);

        if (hasSelected)
        {
            textName.text = Selected.Name;

            textDesc.text = Selected.Desc;

            textBuyCost.text = Selected.CostBuy.ToString();

            textSellCost.text = Selected.CostSell.ToString();

            imageSprite.sprite = Selected.Icon;

            foreach (var transform in transformRecipeLayers)
            {
                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }
            }

            CreateRecipeTree(Selected, 0);
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
        ShopManager.Instance.Sell(Selected);
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
