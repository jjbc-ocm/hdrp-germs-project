using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class GreenMercyManager : SkillBaseManager
{
    [SerializeField]
    private float sustainDelay;

    [SerializeField]
    private float radius;

    private float lastAttackTime;

    protected override void OnInitialize()
    {

    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (Time.time > lastAttackTime + sustainDelay)
        {
            lastAttackTime = Time.time;

            var colliders = Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask("Ship"));

            foreach (var collider in colliders)
            {
                var player = collider.GetComponent<Player>();

                if (!IsHit(owner, player)) continue;

                player.photonView.RPC("RpcDamageHealth", RpcTarget.All, damage, owner.photonView.ViewID);
            }
        }
    }
}
