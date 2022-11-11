using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmellsLikeDeathManager : SkillBaseManager
{
    protected override void OnInitialize()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        autoTarget.photonView.RPC("RpcDamageHealth", RpcTarget.All, damage, owner.photonView.ViewID);
    }
}
