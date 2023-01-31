using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class SupremacyWard : ItemEffectManager
{
    public override void Execute(int slotIndex, Player user)
    {
        user.ItemAim.Aim((targetPosition) =>
        {
            var supremacyWard = PhotonNetwork.InstantiateRoomObject("Supremacy Ward", targetPosition, Quaternion.identity);

            supremacyWard.GetComponent<SupremacyWardEffectManager>().Team = user.GetTeam();

            user.Inventory.TryRemoveItem(slotIndex);
        });
    }
}
