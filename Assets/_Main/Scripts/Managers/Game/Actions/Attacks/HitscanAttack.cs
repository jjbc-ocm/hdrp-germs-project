using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitscanAttack : AttackBase
{
    [SerializeField]
    private FireType fire;

    #region Reticle

    public float gravity = 9.81f;  // the gravity force
    public float travelTime = 1f;  // the time it takes for the projectile to reach the target

    private Vector3 aimDirection;  // the aim direction of the projectile
    private float initialSpeed;  // the initial speed of the projectile
    private Vector3 velocity3D;  // the velocity of the projectile

    #endregion

    #region Override

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


        if (fire == FireType.Straight)
        {

        }

        else if (fire == FireType.Reticle)
        {
            InitializeReticleFire();
        }

        else if (fire == FireType.Sight)
        {

        }
    }

    protected override void OnGet()
    {
        foreach (var trail in trails)
        {
            trail.enabled = false;

            trail.Clear();

            trail.enabled = true;
        }
    }

    protected override void OnRelease()
    {

    }

    #endregion

    private void Update()
    {
        if (fire == FireType.Straight)
        {
            UpdateStraightFire();
        }

        else if (fire == FireType.Reticle)
        {
            UpdateReticleFire();
        }

        else if (fire == FireType.Sight)
        {
            UpdateSightFire();
        }
        
        
    }

    private void InitializeReticleFire()
    {
        // get the direction from the projectile to the target
        Vector3 targetDirection = targetPosition - transform.position;
        // calculate the distance to the target
        float targetDistance = targetDirection.magnitude;
        // calculate the time required for the projectile to hit the target
        float timeToTarget = Mathf.Sqrt(2f * targetDistance / gravity);
        // calculate the initial speed required to hit the target
        initialSpeed = targetDistance / timeToTarget;
        initialSpeed *= 1f;
        // calculate the angle required to hit the target
        float angle = Mathf.Atan((targetDirection.y + 0.5f * gravity * timeToTarget * timeToTarget) / targetDistance);
        // calculate the aim direction of the projectile
        aimDirection = targetDirection.normalized;
        aimDirection.y = Mathf.Tan(angle);
        // calculate the velocity of the projectile
        velocity3D = initialSpeed * aimDirection;
    }

    private void UpdateStraightFire()
    {
        transform.Translate(Vector3.forward * velocity * Time.deltaTime, Space.Self);

        if (Vector3.Dot(transform.forward, targetPosition - transform.position) < 0)
        {
            transform.position = targetPosition;

            Destroy(gameObject);
            //PoolManager.Instance.Release(this);
        }
    }

    private void UpdateReticleFire()
    {
        // calculate the new position of the projectile
        Vector3 newPosition = transform.position + velocity3D * Time.deltaTime;
        newPosition.y += 0.5f * gravity * Time.deltaTime * Time.deltaTime;
        velocity3D.y -= gravity * Time.deltaTime;
        // update the position and velocity of the projectile
        transform.position = newPosition;

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            Destroy(gameObject);
            //PoolManager.Instance.Release(this);
        }


        /* Draw traectory */
        /*Vector3[] points = new Vector3[20];
        float timeStep = travelTime / (float)20;
        for (int i = 0; i < 20; i++)
        {
            float t = i * timeStep;
            Vector3 point = transform.position + initialSpeed * aimDirection * t;
            point.y = transform.position.y + initialSpeed * aimDirection.y * t - 0.5f * gravity * t * t;
            points[i] = point;
        }
        // draw the trajectory line
        for (int i = 0; i < 20 - 1; i++)
        {
            Debug.DrawLine(points[i], points[i + 1], Color.red, 0.1f);
        }*/
    }

    private void UpdateSightFire()
    {

    }
}
