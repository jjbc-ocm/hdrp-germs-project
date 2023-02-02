using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class SmellsLikeDeathSkill : SkillBase
{
    protected override void OnInitialize()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        AudioManager.Instance.Play3D(data.Sounds[0], transform.position);

        ApplyEffect(owner, targetActor);
    }
}
