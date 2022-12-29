
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [SerializeField]
    private ShopUI ui;

    [SerializeField]
    private List<ItemData> data;

    public ShopUI UI { get => ui; }

    public List<ItemData> Data { get => data; }

    void Awake()
    {
        Instance = this;
    }

    public void ToggleShop()
    {
        if (ui.gameObject.activeSelf)
        {
            ui.Close();
        }
        else
        {
            ui.Open((self) =>
            {
                self.Data = data;
            });
        }
    }

    public void CloseShop()
    {
        ui.Close();
    }

    public void Buy(ItemData item)
    {
        var usedSlotIndexes = new List<int>();

        var totalCost = GetTotalCost(item, usedSlotIndexes);

        foreach (var usedSlotIndex in usedSlotIndexes)
        {
            Player.Mine.Inventory.TryRemoveItem(usedSlotIndex);
        }

        if (Player.Mine.Inventory.TryAddItem(item))
        {
            Player.Mine.Inventory.AddGold(-totalCost);

            ui.RefreshUI((self) =>
            {
                self.SelectedData = null;

                self.SelectedSlotIndex = -1;
            });
        }
    }

    public void Sell(int slotIndex)
    {
        if (Player.Mine.Inventory.TryRemoveItem(slotIndex))
        {
            var item = Player.Mine.Inventory.Items[slotIndex];

            Player.Mine.Inventory.AddGold(item.CostSell);

            ui.RefreshUI((self) =>
            {
                self.SelectedData = null;

                self.SelectedSlotIndex = -1;
            });
        }
    }

    public int GetTotalCost(ItemData item, List<int> invSlotCheckedIndexes = null)
    {
        if (invSlotCheckedIndexes == null) invSlotCheckedIndexes = new List<int>();

        var cost = item.CostBuy;

        foreach (var recipe in item.Recipes)
        {
            if (IsInInventory(recipe, invSlotCheckedIndexes))
            {
                cost -= GetTotalCost(recipe, invSlotCheckedIndexes);
            }

        }

        return cost;
    }

    private bool IsInInventory(ItemData item, List<int> invSlotCheckedIndexes)
    {
        for (var i = 0; i < Player.Mine.Inventory.Items.Count; i++)
        {
            var slotItem = Player.Mine.Inventory.Items[i];

            if (!invSlotCheckedIndexes.Contains(i) && slotItem != null && item.ID == slotItem.ID)
            {
                invSlotCheckedIndexes.Add(i);

                return true;
            } 
        }

        return false;
    }
}
