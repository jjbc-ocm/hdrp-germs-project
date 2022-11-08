using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningColdManager : SkillBaseManager
{
    [SerializeField]
    private GameObject vfx;

    void Update()
    {
        //vfx.transform.position = autoTarget.transform.position;
    }

    protected override void OnInitialize()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        autoTarget.TakeDamage(this);

        //autoTarget.photonView.RPC("RpcHpDamage", RpcTarget.All, damage, owner.photonView.ViewID);
    }
}
