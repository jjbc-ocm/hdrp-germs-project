using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class GreenMercySkill : SkillBase
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
        //if (!PhotonNetwork.IsMasterClient) return;

        if (Time.time > lastAttackTime + sustainDelay)
        {
            lastAttackTime = Time.time;

            //var constants = SOManager.Instance.Constants;

            /* Get all ships within range to verify */
            //var layerMask = LayerMask.GetMask(constants.LayerAlly, constants.LayerEnemy, constants.LayerMonster);

            var colliders = Physics.OverlapSphere(transform.position, radius);

            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out ActorManager actor))
                {
                    if (!IsHit(owner, actor)) continue;

                    ApplyEffect(owner, actor);
                }
                
            }

            AudioManager.Instance.Play3D(data.Sounds[0], transform.position);
        }
    }
}
