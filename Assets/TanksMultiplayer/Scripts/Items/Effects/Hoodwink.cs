using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class Hoodwink : ItemEffectManager
{
    public override void Execute(ItemData item, Player user, Vector3 targetLocation)
    {
        user.Status.TryApplyItem(item);

        user.Inventory.TryRemoveItem(item);

        //user.Status.AddStatusGroup(item.StatModifier.CreateInstance());

        //user.photonView.ConsumeItem(item);
    }
}
