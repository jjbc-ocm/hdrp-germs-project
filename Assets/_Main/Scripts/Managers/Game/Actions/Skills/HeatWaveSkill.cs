using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class HeatWaveSkill : SkillBase
{
    [SerializeField]
    private float radius;

    protected override void OnInitialize()
    {
        //if (!PhotonNetwork.IsMasterClient) return;

        AudioManager.Instance.Play3D(data.Sounds[0], transform.position);

        //var constants = SOManager.Instance.Constants;

        //var layerMask = LayerMask.GetMask(constants.LayerAlly, constants.LayerEnemy, constants.LayerMonster);

        var colliders = Physics.OverlapSphere(targetActor.transform.position, radius);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out ActorManager actor))
            {
                if (!IsHit(owner, actor)) continue;

                ApplyEffect(owner, actor);
            } 
        }
    }
}
