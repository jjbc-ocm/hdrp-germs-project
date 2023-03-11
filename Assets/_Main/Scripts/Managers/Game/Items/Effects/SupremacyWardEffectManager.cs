using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupremacyWardEffectManager : MonoBehaviourPunCallbacks
{
    private int team;

    public int Team { get => team; set => team = value; }

    private void Start()
    {
        GameManager.Instance.CacheSupremacyWard(this);

        StartCoroutine(YieldDestroy());
    }

    private IEnumerator YieldDestroy()
    {
        yield return new WaitForSeconds(120);

        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.Instance.UncacheSupremacyWard(this);

            PhotonNetwork.Destroy(photonView);
        }
    }
}
