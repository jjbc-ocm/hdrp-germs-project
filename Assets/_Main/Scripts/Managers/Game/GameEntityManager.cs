using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public abstract class GameEntityManager : MonoBehaviourPunCallbacks
{
    private bool isInsideBush;

    protected abstract void OnTriggerEnterCalled(Collider col);

    protected abstract void OnTriggerExitCalled(Collider col);

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Bush"))
        {
            isInsideBush = true;
        }

        OnTriggerEnterCalled(col);
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Bush"))
        {
            isInsideBush = false;
        }

        OnTriggerExitCalled(col);
    }

    public bool IsVisibleRelativeTo(Transform target)
    {
        var isInTargetRange = Vector3.Distance(transform.position, target.position) <= SOManager.Instance.Constants.FogOrWarDistance;

        if (target.TryGetComponent(out Player targetPlayer))
        {
            // TODO: still have to handle invisibility items
            //return (isInTargetRange || IsInSupremacyWard()) && !isInsideBush;
        }

        return (isInTargetRange || IsInSupremacyWard()) && !isInsideBush;
    }

    protected bool IsInSupremacyWard()
    {
        if (GameManager.Instance.SupremacyWards == null) return false;

        foreach (var supremacyWard in GameManager.Instance.SupremacyWards)
        {
            var distance = Vector3.Distance(transform.position, supremacyWard.transform.position);

            if (distance <= 25) // TODO: do not hard-code this
            {
                return true;
            }
        }

        return false;
    }
}
