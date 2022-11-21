using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public abstract class ItemEffectManager
{
    public abstract void Execute(Player user, Player target);
}
