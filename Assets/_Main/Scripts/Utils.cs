using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TanksMP;
using UnityEngine;
using UnityEngine.AI;

public static class Utils
{
    public static bool IsState(this Animator animator, int layerIndex, string name)
    {
        return animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(name);
    }

    public static bool HasReachedDestinationOrGaveUp(this NavMeshAgent meshAgent)
    {

        if (!meshAgent.pathPending)
        {
            if (meshAgent.remainingDistance <= meshAgent.stoppingDistance)
            {
                if (!meshAgent.hasPath || meshAgent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static void SetLayerRecursive(this GameObject gameObject, int layer)
    {
        gameObject.layer = layer;

        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.SetLayerRecursive(layer);
        }
    }
}
