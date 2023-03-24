
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    [SerializeField]
    private ShopUI ui;

    [SerializeField]
    private List<ItemSO> data;

    public ShopUI UI { get => ui; }

    public List<ItemSO> Data { get => data; }

    #region Unity

    private void Update()
    {
        // Automatically close the shop when player is outside the base
        if (!GameManager.Instance.GetBase(PlayerManager.Mine.GetTeam()).HasPlayer(PlayerManager.Mine) && ui.gameObject.activeSelf)
        {
            ui.Close();
        }
    }

    #endregion

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

    public void Buy(PlayerManager player, ItemSO item)
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

    public void Sell(PlayerManager player, int slotIndex)
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

    public int GetTotalCost(PlayerManager player, ItemSO item, List<int> invSlotCheckedIndexes = null)
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

    public bool CanBuy(PlayerManager player, ItemSO item)
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

        var conditionGold = player.Inventory.Gold >= totalCost;

        var conditionSlot = (freeSlots + usedSlotIndexes.Count) > 0;

        /* 0 purchase limit means infinte */
        var conditionLimit = player.Inventory.GetQuantity(item) < item.PurchaseLimit || item.PurchaseLimit == 0;

        return conditionGold && conditionSlot && conditionLimit;
    }

    private bool IsInInventory(PlayerManager player, ItemSO item, List<int> invSlotCheckedIndexes)
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
