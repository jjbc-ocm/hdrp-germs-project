using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class SupremacyWard : ItemEffectManager
{
    public override void Execute(ItemData item, Player user)
    {
        user.ItemAim.Aim((targetPosition) =>
        {
            PhotonNetwork.InstantiateRoomObject("Supremacy Ward", targetPosition, Quaternion.identity);

            user.Inventory.TryRemoveItem(item);
        });
    }
}
