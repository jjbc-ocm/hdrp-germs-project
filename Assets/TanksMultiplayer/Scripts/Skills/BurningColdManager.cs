using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningColdManager : SkillBaseManager
{
    protected override void OnInitialize()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        autoTarget.TakeDamage(this);
    }
}
