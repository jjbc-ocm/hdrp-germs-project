using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TanksMP;
using UnityEngine;
using UnityEngine.AI;

public static class Utils
{
    public static IEnumerable<GameEntityManager> GetEntityInRange(this Transform transform, float radius)
    {
        var layers = LayerMask.GetMask(
            SOManager.Instance.Constants.LayerAlly,
            SOManager.Instance.Constants.LayerEnemy,
            SOManager.Instance.Constants.LayerMonster);

        var actors = Physics.OverlapSphere(transform.position, radius, layers);

        return actors
            .Select(i => i.GetComponent<GameEntityManager>())
            .Where(i => i.IsVisibleRelativeTo(transform));
        // TODO: need some adjustment in future, must add invisibility constraints too
            /*.Where(i =>
            {
                i.TryGetComponent(out Player player);

                i.TryGetComponent()

                return true;
            });*/
    }

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
