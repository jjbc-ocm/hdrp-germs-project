using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : Singleton<DamageManager>
{
    public void ApplyDamage(ActorManager user, ActorManager target, int damage, bool hasLifeSteal)
    {
        if (PlayerManager.Mine == user || PlayerManager.Mine == target)
        {
            PopupManager.Instance.ShowDamage(damage, target.transform.position);
        }

        if (!PhotonNetwork.IsMasterClient) return;

        var rpcTarget = target is GPMonsterBase ? RpcTarget.MasterClient : RpcTarget.All;

        target.photonView.RPC("RpcDamageHealth", rpcTarget, damage, user.photonView.ViewID);

        if (hasLifeSteal && user.TryGetComponent(out PlayerManager playerUser))
        {
            var lifeSteal = -Mathf.Max(1, Mathf.RoundToInt(damage * playerUser.Inventory.StatModifier.LifeSteal));

            playerUser.photonView.RPC("RpcDamageHealth", RpcTarget.All, lifeSteal, 0);
        }
    }
}
