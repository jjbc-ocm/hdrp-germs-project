
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
        if (Player.Mine.Inventory.TryAddItem(item))
        {
            Player.Mine.Inventory.AddGold(-item.CostBuy);
        }
    }

    public void Sell(ItemData item)
    {
        if (Player.Mine.Inventory.TryRemoveItem(item))
        {
            Player.Mine.Inventory.AddGold(item.CostSell);
        }
    }
}
