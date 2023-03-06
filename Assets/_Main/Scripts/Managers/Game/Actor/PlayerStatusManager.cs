using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TanksMP;
using UnityEngine;

public class PlayerStatusManager : MonoBehaviourPunCallbacks, IPunObservable
{
    private string itemId0;
    private string itemId1;
    private string itemId2;
    private string itemId3;
    private string itemId4;

    private float duration0;
    private float duration1;
    private float duration2;
    private float duration3;
    private float duration4;

    private List<ItemSO> items;

    private StatModifier statModifier;

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
        };

        statModifier = items
            .Where(i => i != null)
            .Select(i => i.StatModifier.CreateInstance())
            .Aggregate(new StatModifier(), (a, b) => a + b);

        if (!photonView.IsMine) return;

        duration0 = Mathf.Max(0, duration0 - Time.deltaTime);
        duration1 = Mathf.Max(0, duration1 - Time.deltaTime);
        duration2 = Mathf.Max(0, duration2 - Time.deltaTime);
        duration3 = Mathf.Max(0, duration3 - Time.deltaTime);
        duration4 = Mathf.Max(0, duration4 - Time.deltaTime);

        if (duration0 <= 0) itemId0 = "";
        if (duration1 <= 0) itemId1 = "";
        if (duration2 <= 0) itemId2 = "";
        if (duration3 <= 0) itemId3 = "";
        if (duration4 <= 0) itemId4 = "";
    }

    #endregion


    public bool TryApplyItem(ItemSO data)
    {
        if (string.IsNullOrEmpty(itemId0)) { itemId0 = data.ID; duration0 += data.Duration; return true; }
        if (string.IsNullOrEmpty(itemId1)) { itemId1 = data.ID; duration1 += data.Duration; return true; }
        if (string.IsNullOrEmpty(itemId2)) { itemId2 = data.ID; duration2 += data.Duration; return true; }
        if (string.IsNullOrEmpty(itemId3)) { itemId3 = data.ID; duration3 += data.Duration; return true; }
        if (string.IsNullOrEmpty(itemId4)) { itemId4 = data.ID; duration4 += data.Duration; return true; }

        return false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(itemId0);
            stream.SendNext(itemId1);
            stream.SendNext(itemId2);
            stream.SendNext(itemId3);
            stream.SendNext(itemId4);
            stream.SendNext(duration0);
            stream.SendNext(duration1);
            stream.SendNext(duration2);
            stream.SendNext(duration3);
            stream.SendNext(duration4);
        }
        else
        {
            itemId0 = (string)stream.ReceiveNext();
            itemId1 = (string)stream.ReceiveNext();
            itemId2 = (string)stream.ReceiveNext();
            itemId3 = (string)stream.ReceiveNext();
            itemId4 = (string)stream.ReceiveNext();
            duration0 = (float)stream.ReceiveNext();
            duration1 = (float)stream.ReceiveNext();
            duration2 = (float)stream.ReceiveNext();
            duration3 = (float)stream.ReceiveNext();
            duration4 = (float)stream.ReceiveNext();
        }
    }
}