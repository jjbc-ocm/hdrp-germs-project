using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class Hoodwink : ItemEffectManager
{
    public override void Execute(int slotIndex, Player user)
    {
        user.Status.TryApplyItem(Player.Mine.Inventory.Items[slotIndex]);

        user.Inventory.TryRemoveItem(slotIndex);
    }
}
