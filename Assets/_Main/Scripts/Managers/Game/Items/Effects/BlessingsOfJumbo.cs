using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class BlessingsOfJumbo : ItemEffectManager
{
    public override void Execute(int slotIndex, PlayerManager user)
    {
        var constants = SOManager.Instance.Constants;

        var layerMask = LayerMask.GetMask(constants.LayerEnemy, constants.LayerMonster);

        var colliders = Physics.OverlapSphere(user.transform.position, 25, layerMask);

        foreach (var collider in colliders)
        {
            var actor = collider.GetComponent<ActorManager>();

            if (IsHit(user, actor))
            {
                /*var damage = 100;

                actor.photonView.RPC("RpcDamageHealth", RpcTarget.All, damage, user.photonView.ViewID);

                var lifeSteal = -Mathf.Max(1, Mathf.RoundToInt(damage * user.Inventory.StatModifier.LifeSteal));

                user.photonView.RPC("RpcDamageHealth", RpcTarget.All, lifeSteal, 0);*/

                //ApplyDamage(100, user, actor);

                DamageManager.Instance.ApplyDamage(user, actor, 100, false);
            }
        }

        user.Inventory.TryRemoveItem(slotIndex);

        InitializeVFX(user.transform.position);
    }

    private void InitializeVFX(Vector3 position)
    {
        PhotonNetwork.InstantiateRoomObject("Blessing of Jumbo", position, Quaternion.identity);
    }

    /*private void ApplyDamage(int damage, ActorManager user, ActorManager target)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        var rpcTarget = target is GPMonsterBase ? RpcTarget.MasterClient : RpcTarget.All;

        target.photonView.RPC("RpcDamageHealth", rpcTarget, damage, user.photonView.ViewID);
    }*/

    private bool IsHit(ActorManager origin, ActorManager target)
    {
        if (target == origin || target == null) return false;

        if ((origin.IsMonster && !target.IsMonster) || (!origin.IsMonster && target.IsMonster)) return true;

        else if (origin.GetTeam() == target.GetTeam()) return false;

        return true;
    }
}
