using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatWaveManager : SkillBaseManager
{
    [SerializeField]
    private GameObject prefabDebugBox; // TODO: delete once there's a VFX

    void Update()
    {
        prefabDebugBox.transform.position = autoTarget.transform.position;
    }

    protected override void OnInitialize()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        autoTarget.TakeDamage(this);
    }
}
