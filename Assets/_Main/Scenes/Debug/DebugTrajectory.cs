using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DebugTrajectory : MonoBehaviour
{
    public Transform target;  // the target object to hit
    public float gravity = 9.81f;  // the gravity force
    public float travelTime = 2f;  // the time it takes for the projectile to reach the target
    public float lineWidth = 0.1f;  // the width of the trajectory line
    public int lineSegments = 20;  // the number of line segments to draw

    private Vector3 aimDirection;  // the aim direction of the projectile
    private float initialSpeed;  // the initial speed of the projectile

    /*void Start()
    {
        CalculateAim();
    }*/

    void Update()
    {
        CalculateAim();

        DrawTrajectory();
    }

    void CalculateAim()
    {
        // get the direction from the projectile to the target
        Vector3 targetDirection = target.position - transform.position;
        // get the horizontal distance to the target
        float targetDistance = new Vector3(targetDirection.x, 0f, targetDirection.z).magnitude;
        // calculate the initial speed required to hit the target
        initialSpeed = targetDistance / travelTime;
        // calculate the angle required to hit the target
        float angle = Mathf.Atan((targetDirection.y + 0.5f * gravity * travelTime * travelTime) / targetDistance);
        // calculate the aim direction of the projectile
        aimDirection = targetDirection.normalized;
        aimDirection.y = Mathf.Tan(angle);
    }

    void DrawTrajectory()
    {
        // calculate the trajectory points
        Vector3[] points = new Vector3[lineSegments];
        float timeStep = travelTime / (float)lineSegments;
        for (int i = 0; i < lineSegments; i++)
        {
            float t = i * timeStep;
            Vector3 point = transform.position + initialSpeed * aimDirection * t;
            point.y = transform.position.y + initialSpeed * aimDirection.y * t - 0.5f * gravity * t * t;
            points[i] = point;
        }
        // draw the trajectory line
        for (int i = 0; i < lineSegments - 1; i++)
        {
            Debug.DrawLine(points[i], points[i + 1], Color.red, lineWidth);
        }
    }
}
