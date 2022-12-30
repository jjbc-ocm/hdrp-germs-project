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

            var colliders = Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask("Ship", "Monster"));

            foreach (var collider in colliders)
            {
                var actor = collider.GetComponent<ActorManager>();

                if (!IsHit(owner, actor)) continue;

                actor.photonView.RPC("RpcDamageHealth", RpcTarget.All, damage, owner.photonView.ViewID);

                if (owner is Player)
                {
                    var lifeSteal = -Mathf.Max(1, Mathf.RoundToInt(damage * (owner as Player).Inventory.StatModifier.LifeSteal));

                    owner.photonView.RPC("RpcDamageHealth", RpcTarget.All, lifeSteal, 0);
                }
            }
        }
    }
}
