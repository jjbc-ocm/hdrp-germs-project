using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class BlessingsOfJumbo : ItemEffectManager
{
    public override void Execute(ItemData item, Player user, Vector3 targetLocation)
    {
        // TODO: radius need to be modifieable
        var colliders = Physics.OverlapSphere(user.transform.position, 25, LayerMask.GetMask("Ship"));

        foreach (var collider in colliders)
        {
            var player = collider.GetComponent<Player>();

            if (!IsHit(user, player)) continue;

            // TODO: damage should not be hard coded
            player.photonView.RPC("RpcDamageHealth", RpcTarget.All, 100, user.photonView.ViewID); 
        }

        user.photonView.ConsumeItem(item);
    }

    private bool IsHit(ActorManager origin, ActorManager target)
    {
        if (target == origin || target == null) return false;

        if ((origin.IsMonster && !target.IsMonster) || (!origin.IsMonster && target.IsMonster)) return true;

        else if (origin.photonView.GetTeam() == target.photonView.GetTeam()) return false;

        return true;
    }
}
