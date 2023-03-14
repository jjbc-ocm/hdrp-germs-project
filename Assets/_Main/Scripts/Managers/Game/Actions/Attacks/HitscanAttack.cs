using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitscanAttack : AttackBase
{
    protected override void OnInitialize()
    {
        var fromPosition = owner.transform.position;

        var direction = (targetPosition - fromPosition).normalized;

        var maxDistance = SOManager.Instance.Constants.FogOrWarDistance;

        var layerMask = Utils.GetBulletHitMask(owner.gameObject);

        direction = new Vector3(direction.x, 0, direction.z).normalized;

        transform.position = fromPosition + Vector3.up * 2;

        transform.forward = direction;

        targetPosition = fromPosition + direction * SOManager.Instance.Constants.FogOrWarDistance + Vector3.up * 2;

        if (Physics.Raycast(fromPosition, direction, out RaycastHit hit, maxDistance, layerMask))
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

    
}
