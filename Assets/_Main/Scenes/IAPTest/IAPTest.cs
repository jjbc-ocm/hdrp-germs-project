using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.GameFoundation;

public class IAPTest : MonoBehaviour
{
    private InventoryDef[] shopItems;

    private void Start()
    {
        try
        {
            SteamClient.Init(2261610);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        InitializeShopItems();


        SteamUser.OnMicroTxnAuthorizationResponse += OnPurchaseFinished;


    }






    public void BuyGem()
    {
        CheckoutAsync(0);
    }

    private async void InitializeShopItems()
    {
        shopItems = await SteamInventory.GetDefinitionsWithPricesAsync();

        foreach (var shopItem in shopItems)
        {
            Debug.Log(shopItem.Name + " " + shopItem.LocalPrice);
        }
    }

    private async void CheckoutAsync(int index)
    {
        // This tries to open the steam overlay to commence the checkout
        var result = await SteamInventory.StartPurchaseAsync(new InventoryDef[] { shopItems[index] });

        Debug.Log($"Result: {result.Value.Result}");
        Debug.Log($"TransID: {result.Value.TransID}");
        Debug.Log($"OrderID: {result.Value.OrderID}");
    }

    private void OnPurchaseFinished(AppId appid, ulong orderid, bool success)
    {
        if (success)
        {
            Debug.Log(appid + " " + orderid);
        }
        else
        {
            Debug.LogError("OnPurchaseFinished failed");
            // They probably pressed cancel or something
        }
    }
}
