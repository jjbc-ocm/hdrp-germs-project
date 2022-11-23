using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public abstract class ItemEffectManager
{
    public abstract void Execute(ItemData item, Player user, Vector3 targetLocation);
}
