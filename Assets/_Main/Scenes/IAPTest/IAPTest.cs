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
            SteamClient.Init(SOManager.Instance.Constants.AppID);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        InitializeShopItems();


        

    }

    private async void InitializeShopItems()
    {
        shopItems = await SteamInventory.GetDefinitionsWithPricesAsync();

        foreach (var shopItem in shopItems)
        {
            Debug.Log(shopItem.Name + " " + shopItem.LocalPrice);
        }
    }
}
