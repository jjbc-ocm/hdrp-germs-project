using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupremacyWardEffectManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        StartCoroutine(YieldDestroy());
    }

    private IEnumerator YieldDestroy()
    {
        yield return new WaitForSeconds(120);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(photonView);
        }
    }
}
