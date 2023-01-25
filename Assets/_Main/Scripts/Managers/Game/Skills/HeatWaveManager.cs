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

        AudioManager.Instance.Play3D(data.Sounds[0], transform.position);

        var constants = SOManager.Instance.Constants;

        var layerMask = LayerMask.GetMask(constants.LayerEnemy, constants.LayerMonster);

        var colliders = Physics.OverlapSphere(autoTarget.transform.position, radius, layerMask);

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
