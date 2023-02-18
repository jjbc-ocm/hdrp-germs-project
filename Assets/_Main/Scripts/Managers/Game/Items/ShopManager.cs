
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

    public void Buy(Player player, ItemData item)
    {
        var usedSlotIndexes = new List<int>();

        var totalCost = GetTotalCost(player, item, usedSlotIndexes);

        foreach (var usedSlotIndex in usedSlotIndexes)
        {
            player.Inventory.TryRemoveItem(usedSlotIndex);
        }

        if (player.Inventory.TryAddItem(item))
        {
            player.Inventory.AddGold(-totalCost);

            ui.RefreshUI((self) =>
            {
                self.SelectedData = null;

                self.SelectedSlotIndex = -1;
            });
        }
    }

    public void Sell(Player player, int slotIndex)
    {
        if (player.Inventory.TryRemoveItem(slotIndex))
        {
            var item = player.Inventory.Items[slotIndex];

            player.Inventory.AddGold(item.CostSell);

            ui.RefreshUI((self) =>
            {
                self.SelectedData = null;

                self.SelectedSlotIndex = -1;
            });
        }
    }

    public int GetTotalCost(Player player, ItemData item, List<int> invSlotCheckedIndexes = null)
    {
        if (invSlotCheckedIndexes == null) invSlotCheckedIndexes = new List<int>();

        var cost = item.CostBuy;

        foreach (var recipe in item.Recipes)
        {
            if (IsInInventory(player, recipe, invSlotCheckedIndexes))
            {
                cost -= GetTotalCost(player, recipe, invSlotCheckedIndexes);
            }

        }

        return cost;
    }

    public bool CanBuy(Player player, ItemData item)
    {
        var freeSlots =
            (string.IsNullOrEmpty(player.Inventory.ItemId0) ? 1 : 0) +
            (string.IsNullOrEmpty(player.Inventory.ItemId1) ? 1 : 0) +
            (string.IsNullOrEmpty(player.Inventory.ItemId2) ? 1 : 0) +
            (string.IsNullOrEmpty(player.Inventory.ItemId3) ? 1 : 0) +
            (string.IsNullOrEmpty(player.Inventory.ItemId4) ? 1 : 0) +
            (string.IsNullOrEmpty(player.Inventory.ItemId5) ? 1 : 0);

        var usedSlotIndexes = new List<int>();

        var totalCost = GetTotalCost(player, item, usedSlotIndexes);

        return player.Inventory.Gold >= totalCost && (freeSlots + usedSlotIndexes.Count) > 0;
    }

    private bool IsInInventory(Player player, ItemData item, List<int> invSlotCheckedIndexes)
    {
        for (var i = 0; i < player.Inventory.Items.Count; i++)
        {
            var slotItem = player.Inventory.Items[i];

            if (!invSlotCheckedIndexes.Contains(i) && slotItem != null && item.ID == slotItem.ID)
            {
                invSlotCheckedIndexes.Add(i);

                return true;
            } 
        }

        return false;
    }
}
