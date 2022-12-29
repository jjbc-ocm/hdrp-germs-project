using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupremacyWardEffectManager : MonoBehaviourPunCallbacks
{
    private int team;

    public int Team { get => team; set => team = value; }

    void Start()
    {
        StartCoroutine(YieldDestroy());
    }

    private IEnumerator YieldDestroy()
    {
        yield return new WaitForSeconds(10);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(photonView);
        }
    }
}
