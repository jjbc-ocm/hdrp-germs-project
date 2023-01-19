using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

//[RequireComponent(typeof(BoxCollider))]
public class HolyBrightManager : SkillBaseManager
{
    [SerializeField]
    private float sustainDelay;

    [SerializeField]
    private float spawnOffsetDown;

    [SerializeField]
    private float spawnOffsetForward;

    private float lastAttackTime;

    void Update()
    {
        var offset = new Vector3(
            spawnOffsetForward * transform.forward.x,
            -spawnOffsetDown,
            spawnOffsetForward * transform.forward.z);

        transform.position = owner.transform.position + offset;

        if (!PhotonNetwork.IsMasterClient) return;

        if (Time.time > lastAttackTime + sustainDelay)
        {
            lastAttackTime = Time.time;

            var center = transform.position + transform.forward * data.Range / 2f;

            var halfExtents = new Vector3(5, 5, data.Range) / 2f;

            var orientation = Quaternion.Euler(transform.eulerAngles);

            var colliders = Physics.OverlapBox(center, halfExtents, orientation, LayerMask.GetMask("Ship", "Monster"));

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

    protected override void OnInitialize()
    {
        AudioManager.Instance.Play3D(data.Sounds[0], transform.position);
    }

    
}
