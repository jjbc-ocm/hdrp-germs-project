using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class BaseItemEffect : ItemEffectManager
{
    public override void Execute(ItemData item, Player user, Vector3 targetLocation)
    {
        user.Status.AddStatusGroup(item.StatModifier.CreateInstance());
    }
}
