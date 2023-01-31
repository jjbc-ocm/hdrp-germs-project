using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class GameEntityManager : MonoBehaviourPunCallbacks
{
    protected bool IsInSupremacyWard()
    {
        if (GameManager.Instance.SupremacyWards == null) return false;

        foreach (var supremacyWard in GameManager.Instance.SupremacyWards)
        {
            var distance = Vector3.Distance(transform.position, supremacyWard.transform.position);

            if (distance <= 25)
            {
                return true;
            }
        }

        return false;
    }
}
