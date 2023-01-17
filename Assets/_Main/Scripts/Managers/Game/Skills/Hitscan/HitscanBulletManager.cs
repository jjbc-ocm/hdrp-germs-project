using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitscanBulletManager : HitscanManager
{
    [SerializeField]
    private float velocity;
  
    private void Update()
    {
        transform.Translate(Vector3.forward * velocity * Time.deltaTime, Space.Self);

        if (Vector3.Dot(transform.forward, to - transform.position) < 0)
        {
            transform.position = to;

            Destroy(gameObject);
        }
    }
}
