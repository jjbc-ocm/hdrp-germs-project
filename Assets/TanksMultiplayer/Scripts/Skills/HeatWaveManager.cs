using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class HeatWaveManager : SkillBaseManager
{
    [SerializeField]
    private float radius;

    protected override void OnInitialize()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        var colliders = Physics.OverlapSphere(autoTarget.transform.position, radius, LayerMask.GetMask("Ship"));

        foreach (var collider in colliders)
        {
            var player = collider.GetComponent<Player>();

            if (!IsHit(owner, player)) continue;

            player.photonView.RPC("RpcDamageHealth", RpcTarget.All, damage, owner.photonView.ViewID);
        }
    }
}
