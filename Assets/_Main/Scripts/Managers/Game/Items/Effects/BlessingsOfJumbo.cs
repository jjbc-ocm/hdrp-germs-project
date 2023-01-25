using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class BlessingsOfJumbo : ItemEffectManager
{
    public override void Execute(int slotIndex, Player user)
    {
        var constants = SOManager.Instance.Constants;

        var layerMask = LayerMask.GetMask(constants.LayerEnemy, constants.LayerMonster);

        var colliders = Physics.OverlapSphere(user.transform.position, 25, layerMask);

        foreach (var collider in colliders)
        {
            var actor = collider.GetComponent<ActorManager>();

            if (IsHit(user, actor))
            {
                var damage = 100;

                actor.photonView.RPC("RpcDamageHealth", RpcTarget.All, damage, user.photonView.ViewID);

                var lifeSteal = -Mathf.Max(1, Mathf.RoundToInt(damage * user.Inventory.StatModifier.LifeSteal));

                user.photonView.RPC("RpcDamageHealth", RpcTarget.All, lifeSteal, 0);
            }
        }

        user.Inventory.TryRemoveItem(slotIndex);
    }

    private bool IsHit(ActorManager origin, ActorManager target)
    {
        if (target == origin || target == null) return false;

        if ((origin.IsMonster && !target.IsMonster) || (!origin.IsMonster && target.IsMonster)) return true;

        else if (origin.photonView.GetTeam() == target.photonView.GetTeam()) return false;

        return true;
    }
}
