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

    #region Unity

    private void Awake()
    {
        StartCoroutine(YieldCacheSelf());
    }

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

    private void OnDestroy()
    {
        GameManager.Instance.UncacheEntity(this);
    }

    #endregion

    #region Public

    public bool IsVisibleRelativeTo(Transform target)
    {
        var isInTargetRange = Vector3.Distance(transform.position, target.position) <= SOManager.Instance.Constants.FogOrWarDistance;

        if (target.TryGetComponent(out PlayerManager targetPlayer))
        {
            // TODO: still have to handle invisibility items
            //return (isInTargetRange || IsInSupremacyWard()) && !isInsideBush;
        }

        return (isInTargetRange || IsInSupremacyWard()) && !isInsideBush;
    }

    #endregion

    #region Protected

    protected bool IsInSupremacyWard()
    {
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

    #endregion

    #region Private

    private IEnumerator YieldCacheSelf()
    {
        var isCached = false;

        while (!isCached)
        {
            yield return new WaitForEndOfFrame();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.CacheEntity(this);

                isCached = true;
            }
        }
    }

    #endregion
}
