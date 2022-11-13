using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [SerializeField]
    public ItemData[] data;

    void Awake()
    {
        Instance = this;
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
