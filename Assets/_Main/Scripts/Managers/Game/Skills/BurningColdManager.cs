using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class BurningColdManager : SkillBaseManager
{
    protected override void OnInitialize()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        autoTarget.photonView.RPC("RpcDamageHealth", RpcTarget.All, damage, owner.photonView.ViewID);

        if (owner is Player)
        {
            var lifeSteal = -Mathf.Max(1, Mathf.RoundToInt(damage * (owner as Player).Inventory.StatModifier.LifeSteal));

            owner.photonView.RPC("RpcDamageHealth", RpcTarget.All, lifeSteal, 0);
        }
    }
}
