using Photon.Pun;
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

    void Awake()
    {
        Instance = this;
    }

    public void OpenShop()
    {
        ui.Open((self) =>
        {
            self.Data = data;
        });
    }

    public void CloseShop()
    {
        ui.Close();
    }

    public void Buy(ItemData item)
    {
        PhotonNetwork.LocalPlayer.PurchaseItem(item, 1);
    }

    public void Sell(ItemData item)
    {
        PhotonNetwork.LocalPlayer.PurchaseItem(item, -1);
    }
}
