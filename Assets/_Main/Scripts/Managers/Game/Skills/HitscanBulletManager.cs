using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitscanBulletManager : SkillBaseManager
{
    protected override void OnInitialize()
    {

    }

    /*private void Update()
    {
        //if (hasHit) return;

        var velocity = 250f; // TODO: it should not be hard-coded

        transform.Translate(Vector3.forward * velocity * Time.deltaTime, Space.Self);

        *//*if (Vector3.Dot(transform.forward, endPoint - transform.position) < 0)
        {
            transform.position = endPoint;

            Destroy(gameObject);
        }*//*
    }*/
}
