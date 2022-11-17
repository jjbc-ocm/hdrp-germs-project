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

            var colliders = Physics.OverlapBox(center, halfExtents, orientation, LayerMask.GetMask("Ship"));

            foreach (var collider in colliders)
            {
                var player = collider.GetComponent<Player>();

                if (!IsHit(owner, player)) continue;
                
                player.photonView.RPC("RpcDamageHealth", RpcTarget.All, damage, owner.photonView.ViewID);
            }
        }
    }

    protected override void OnInitialize()
    {

    }

    
}
