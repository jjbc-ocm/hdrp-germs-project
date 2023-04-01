using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TanksMP;
using UnityEngine;
using UnityEngine.AI;

/*class Debug : UnityEngine.Debug
{
    public static void DrawCircle(Vector3 position, float radius, float segments, Color color, float duration = 0)
    {
        // If either radius or number of segments are less or equal to 0, skip drawing
        if (radius <= 0.0f || segments <= 0)
        {
            return;
        }

        // Single segment of the circle covers (360 / number of segments) degrees
        float angleStep = (360.0f / segments);

        // Result is multiplied by Mathf.Deg2Rad constant which transforms degrees to radians
        // which are required by Unity's Mathf class trigonometry methods

        angleStep *= Mathf.Deg2Rad;

        // lineStart and lineEnd variables are declared outside of the following for loop
        Vector3 lineStart = Vector3.zero;
        Vector3 lineEnd = Vector3.zero;

        for (int i = 0; i < segments; i++)
        {
            // Line start is defined as starting angle of the current segment (i)
            lineStart.x = Mathf.Cos(angleStep * i);
            lineStart.y = Mathf.Sin(angleStep * i);

            // Line end is defined by the angle of the next segment (i+1)
            lineEnd.x = Mathf.Cos(angleStep * (i + 1));
            lineEnd.y = Mathf.Sin(angleStep * (i + 1));

            // Results are multiplied so they match the desired radius
            lineStart *= radius;
            lineEnd *= radius;

            // Results are offset by the desired position/origin 
            lineStart += position;
            lineEnd += position;

            // Points are connected using DrawLine method and using the passed color
            DrawLine(lineStart, lineEnd, color, duration);
        }
    }
}*/

public static class Utils
{
    public static LayerMask GetBulletHitMask(GameObject gameObject) // TODO: found a way to make usable in other skills
    {
        // TODO: limitation, it always refer to target the enemy
        var constants = SOManager.Instance.Constants;

        var isOwnerAlly = gameObject.layer == LayerMask.NameToLayer(constants.LayerAlly);

        //var isOwnerEnemy = owner.gameObject.layer == LayerMask.NameToLayer(constants.LayerEnemy);

        var layerOpposingTeam = isOwnerAlly ? constants.LayerEnemy : constants.LayerAlly;

        return LayerMask.GetMask(layerOpposingTeam, constants.LayerMonster, constants.LayerEnvironment);
    }

    /*public static void CheckBulletRaycast(this GameObject gameObject, ActorManager owner, ref Vector3 targetPosition, Action<RaycastHit> OnHit)
    {
        var fromPosition = owner.transform.position;

        var direction = (targetPosition - fromPosition).normalized;

        var maxDistance = SOManager.Instance.Constants.FogOrWarDistance;

        var layerMask = GetBulletHitMask(owner.gameObject);

        direction = new Vector3(direction.x, 0, direction.z).normalized;

        gameObject.transform.position = fromPosition + Vector3.up * 2;

        gameObject.transform.forward = direction;

        targetPosition = fromPosition + direction * SOManager.Instance.Constants.FogOrWarDistance + Vector3.up * 2;

        if (Physics.Raycast(fromPosition, direction, out RaycastHit hit, maxDistance, layerMask))
        {
            OnHit.Invoke(hit);
        }
    }*/

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
