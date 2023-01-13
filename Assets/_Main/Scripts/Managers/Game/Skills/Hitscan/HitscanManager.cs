using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitscanManager : MonoBehaviour
{
    protected Vector3 from;

    protected Vector3 to;

    public void Initialize(Vector3 from, Vector3 to)
    {
        this.from = from;

        this.to = to;

        transform.position = from;

        transform.forward = (to - from).normalized;
    }
}
