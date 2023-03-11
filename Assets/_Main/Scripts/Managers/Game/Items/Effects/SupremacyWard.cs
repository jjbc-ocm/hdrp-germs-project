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
            var newObject = PhotonNetwork.InstantiateRoomObject("Supremacy Ward", targetPosition, Quaternion.identity);

            var supremacyWard = newObject.GetComponent<SupremacyWardEffectManager>();

            supremacyWard.Team = user.GetTeam();

            user.Inventory.TryRemoveItem(slotIndex);
        });
    }
}
