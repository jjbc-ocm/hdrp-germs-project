using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class HeatWaveManager : SkillBaseManager
{
    //[SerializeField]
    //private GameObject vfx;

    [SerializeField]
    private float radius;

    void Update()
    {
        //vfx.transform.position = autoTarget.transform.position;
    }

    protected override void OnInitialize()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        var colliders = Physics.OverlapSphere(autoTarget.transform.position, radius, LayerMask.GetMask("Ship"));

        foreach (var collider in colliders)
        {
            var player = collider.GetComponent<Player>();

            if (!IsHit(owner, player)) continue;

            //player.TakeDamage(this);

            player.photonView.RPC("RpcDamageHealth", RpcTarget.All, damage, owner.photonView.ViewID);
        }
    }
}
