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

        // TODO: test to mamaya a, kunwari yung RpcDamageHealth tatama lang doon sa player mismo
        //var rpcTarget = target is GPMonsterBase ? RpcTarget.MasterClient : //RpcTarget.All;

        if (target is GPMonsterBase)
        {
            target.photonView.RPC("RpcDamageHealth", RpcTarget.MasterClient, damage, user.photonView.ViewID);
        }
        else
        {
            target.photonView.RPC("RpcDamageHealth", target.photonView.Owner, damage, user.photonView.ViewID);
        }
        
        if (hasLifeSteal && user.TryGetComponent(out PlayerManager playerUser))
        {
            var lifeSteal = -Mathf.Max(1, Mathf.RoundToInt(damage * playerUser.Inventory.StatModifier.LifeSteal));

            //playerUser.photonView.RPC("RpcDamageHealth", RpcTarget.All, lifeSteal, 0);
            playerUser.photonView.RPC("RpcDamageHealth", playerUser.photonView.Owner, lifeSteal, 0);
        }
    }
}
