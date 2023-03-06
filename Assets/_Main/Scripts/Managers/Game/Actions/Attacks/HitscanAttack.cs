using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitscanAttack : AttackBase
{
    protected override void OnInitialize()
    {
        var fromPosition = owner.transform.position;

        var direction = (targetPosition - fromPosition).normalized;

        direction = new Vector3(direction.x, 0, direction.z).normalized;

        transform.position = fromPosition + Vector3.up * 2;

        transform.forward = direction;

        targetPosition = fromPosition + direction * SOManager.Instance.Constants.FogOrWarDistance + Vector3.up * 2;

        if (Physics.Raycast(fromPosition, direction, out RaycastHit hit, SOManager.Instance.Constants.FogOrWarDistance, GetMask()))
        {
            if (hit.transform.TryGetComponent(out ActorManager actor))
            {
                ApplyEffect(owner, actor);
            }

            targetPosition = hit.point;
        }

        AudioManager.Instance.Play3D(data.Sounds[0], transform.position);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * velocity * Time.deltaTime, Space.Self);

        if (Vector3.Dot(transform.forward, targetPosition - transform.position) < 0)
        {
            transform.position = targetPosition;

            Destroy(gameObject);
        }
    }

    private LayerMask GetMask() // TODO: found a way to make usable in other skills
    {
        // TODO: limitation, it always refer to target the enemy
        var constants = SOManager.Instance.Constants;

        var isOwnerAlly = owner.gameObject.layer == LayerMask.NameToLayer(constants.LayerAlly);

        //var isOwnerEnemy = owner.gameObject.layer == LayerMask.NameToLayer(constants.LayerEnemy);

        var layerOpposingTeam = isOwnerAlly ? constants.LayerEnemy : constants.LayerAlly;

        return LayerMask.GetMask(layerOpposingTeam, constants.LayerMonster, constants.LayerEnvironment);
    }
}
