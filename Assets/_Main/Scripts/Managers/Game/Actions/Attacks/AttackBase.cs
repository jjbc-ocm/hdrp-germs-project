using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBase : ActionBase
{
    [SerializeField]
    protected TrailRenderer[] trails;

    [SerializeField]
    protected float velocity;
}
