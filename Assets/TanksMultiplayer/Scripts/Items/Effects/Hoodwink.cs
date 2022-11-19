using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class Hoodwink : ItemEffectManager
{
    public override void Execute(Player user, Player target)
    {
        // Apply invisibility
        user.Stat.IsInvisible = true;

        // apply movement speed

        // TODO: apply timer
    }
}
