using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventoryManager : MonoBehaviourPunCallbacks, IPunObservable
{
    private int gold;

    private string itemId0;
    private string itemId1;
    private string itemId2;
    private string itemId3;
    private string itemId4;
    private string itemId5;

    private List<ItemSO> items;

    private StatModifier statModifier;

    public int Gold { get => gold; set => gold = value; }

    public string ItemId0 { get => itemId0; set => itemId0 = value; }

    public string ItemId1 { get => itemId1; set => itemId1 = value; }

    public string ItemId2 { get => itemId2; set => itemId2 = value; }

    public string ItemId3 { get => itemId3; set => itemId3 = value; }

    public string ItemId4 { get => itemId4; set => itemId4 = value; }

    public string ItemId5 { get => itemId5; set => itemId5 = value; }

    public List<ItemSO> Items { get => items; }

    public StatModifier StatModifier { get => statModifier; }


    #region Unity

    private void Update()
    {
        items = new List<ItemSO>
        {
            ShopManager.Instance.Data.FirstOrDefault(i => i.ID == itemId0),
            ShopManager.Instance.Data.FirstOrDefault(i => i.ID == itemId1),
            ShopManager.Instance.Data.FirstOrDefault(i => i.ID == itemId2),
            ShopManager.Instance.Data.FirstOrDefault(i => i.ID == itemId3),
            ShopManager.Instance.Data.FirstOrDefault(i => i.ID == itemId4),
            ShopManager.Instance.Data.FirstOrDefault(i => i.ID == itemId5)
        };

        statModifier = items
            .Where(i => i != null && i.Category != CategoryType.Consumables)
            .Select(i => i.StatModifier.CreateInstance())
            .Aggregate(new StatModifier(), (a, b) => a + b);
    }

    #endregion

    #region Public

    public bool TryAddItem(ItemSO data)
    {
        if (string.IsNullOrEmpty(itemId0)) { itemId0 = data.ID; return true; }
        if (string.IsNullOrEmpty(itemId1)) { itemId1 = data.ID; return true; }
        if (string.IsNullOrEmpty(itemId2)) { itemId2 = data.ID; return true; }
        if (string.IsNullOrEmpty(itemId3)) { itemId3 = data.ID; return true; }
        if (string.IsNullOrEmpty(itemId4)) { itemId4 = data.ID; return true; }
        if (string.IsNullOrEmpty(itemId5)) { itemId5 = data.ID; return true; }

        return false;
    }

    public bool TryRemoveItem(int slotIndex)
    {
        if (slotIndex == 0) { itemId0 = ""; return true; }
        if (slotIndex == 1) { itemId1 = ""; return true; }
        if (slotIndex == 2) { itemId2 = ""; return true; }
        if (slotIndex == 3) { itemId3 = ""; return true; }
        if (slotIndex == 4) { itemId4 = ""; return true; }
        if (slotIndex == 5) { itemId5 = ""; return true; }

        return false;
    }

    public int GetQuantity(ItemSO item)
    {
        var quantity = 0;

        if (itemId0 == item.ID) { quantity += 1; }
        if (itemId1 == item.ID) { quantity += 1; }
        if (itemId2 == item.ID) { quantity += 1; }
        if (itemId3 == item.ID) { quantity += 1; }
        if (itemId4 == item.ID) { quantity += 1; }
        if (itemId5 == item.ID) { quantity += 1; }


        return quantity;
    }

    #endregion

    #region Photon

    [PunRPC]
    public void AddGold(int amount)
    {
        gold += amount;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(gold);
            stream.SendNext(itemId0);
            stream.SendNext(itemId1);
            stream.SendNext(itemId2);
            stream.SendNext(itemId3);
            stream.SendNext(itemId4);
            stream.SendNext(itemId5);
        }
        else
        {
            gold = (int)stream.ReceiveNext();
            itemId0 = (string)stream.ReceiveNext();
            itemId1 = (string)stream.ReceiveNext();
            itemId2 = (string)stream.ReceiveNext();
            itemId3 = (string)stream.ReceiveNext();
            itemId4 = (string)stream.ReceiveNext();
            itemId5 = (string)stream.ReceiveNext();
        }
    }

    #endregion
}
