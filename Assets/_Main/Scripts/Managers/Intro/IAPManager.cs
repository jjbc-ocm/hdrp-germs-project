using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.GameFoundation;

public class IAPManager : Singleton<IAPManager>
{
    private InventoryDef[] shopItems;

    private bool isSuccess;

    #region Unity

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

        SteamUser.OnMicroTxnAuthorizationResponse += OnPurchaseFinished;

    }

    #endregion

    #region Public

    public async void InitializeShopItems(Action<InventoryDef[]> onComplete)
    {
        shopItems = await SteamInventory.GetDefinitionsWithPricesAsync();

        onComplete.Invoke(shopItems);
    }

    public async void BuyGem(InventoryDef item)
    {
        // This tries to open the steam overlay to commence the checkout
        var result = await SteamInventory.StartPurchaseAsync(new InventoryDef[] { item });

        //var result = await SteamInventory.StartPurchaseAsync(new InventoryDef[] { new InventoryDef(new Steamworks.Data.InventoryDefId { Value = 100 }) });

        Debug.Log($"Result: {result.Value.Result}");
        Debug.Log($"TransID: {result.Value.TransID}");
        Debug.Log($"OrderID: {result.Value.OrderID}");

        if (isSuccess)
        {
            Debug.Log("YOWN PWEDE NA ISAVE SA UGS!");
        }
    }

    #endregion










    private void OnPurchaseFinished(AppId appid, ulong orderid, bool success)
    {
        isSuccess = success;

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
