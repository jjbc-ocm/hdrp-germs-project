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

    private List<ItemData> items;

    private StatModifier statModifier;

    public int Gold { get => gold; set => gold = value; }

    public string ItemId0 { get => itemId0; set => itemId0 = value; }

    public string ItemId1 { get => itemId1; set => itemId1 = value; }

    public string ItemId2 { get => itemId2; set => itemId2 = value; }

    public string ItemId3 { get => itemId3; set => itemId3 = value; }

    public string ItemId4 { get => itemId4; set => itemId4 = value; }

    public string ItemId5 { get => itemId5; set => itemId5 = value; }

    public List<ItemData> Items { get => items; }

    public StatModifier StatModifier { get => statModifier; }

    void Update()
    {
        items = new List<ItemData>
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

    [PunRPC]
    public void AddGold(int amount)
    {
        gold += amount;

        Debug.Log("GOTHEREEEEe " + gold);
    }

    public bool TryAddItem(ItemData data)
    {
        if (string.IsNullOrEmpty(itemId0)) { itemId0 = data.ID; return true; }
        if (string.IsNullOrEmpty(itemId1)) { itemId1 = data.ID; return true; }
        if (string.IsNullOrEmpty(itemId2)) { itemId2 = data.ID; return true; }
        if (string.IsNullOrEmpty(itemId3)) { itemId3 = data.ID; return true; }
        if (string.IsNullOrEmpty(itemId4)) { itemId4 = data.ID; return true; }
        if (string.IsNullOrEmpty(itemId5)) { itemId5 = data.ID; return true; }

        return false;
    }

    /*public bool TryRemoveItem(ItemData data)
    {
        if (itemId0 == data.ID) { itemId0 = ""; return true; }
        if (itemId1 == data.ID) { itemId1 = ""; return true; }
        if (itemId2 == data.ID) { itemId2 = ""; return true; }
        if (itemId3 == data.ID) { itemId3 = ""; return true; }
        if (itemId4 == data.ID) { itemId4 = ""; return true; }
        if (itemId5 == data.ID) { itemId5 = ""; return true; }

        return false;
    }*/

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

    /*public bool HasItem(ItemData data)
    {
        return items.Any(i => i != null && data != null && i.ID == data.ID);
    }*/

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
}
