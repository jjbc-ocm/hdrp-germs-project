using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlessingOfJumboEffectManager : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        StartCoroutine(YieldDestroy());
    }

    private IEnumerator YieldDestroy()
    {
        yield return new WaitForSeconds(5f);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(photonView);
        }
    }
}
