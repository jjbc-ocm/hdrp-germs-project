using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class BlessingsOfJumbo : ItemEffectManager
{
    public override void Execute(ItemData item, Player user)
    {
        var colliders = Physics.OverlapSphere(user.transform.position, 25, LayerMask.GetMask("Ship", "Monster"));

        foreach (var collider in colliders)
        {
            var actor = collider.GetComponent<ActorManager>();

            if (IsHit(user, actor))
            {
                actor.photonView.RPC("RpcDamageHealth", RpcTarget.All, 100, user.photonView.ViewID);
            }
        }

        user.Inventory.TryRemoveItem(item);
    }

    private bool IsHit(ActorManager origin, ActorManager target)
    {
        if (target == origin || target == null) return false;

        if ((origin.IsMonster && !target.IsMonster) || (!origin.IsMonster && target.IsMonster)) return true;

        else if (origin.photonView.GetTeam() == target.photonView.GetTeam()) return false;

        return true;
    }
}
