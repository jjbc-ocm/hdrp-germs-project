using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : ActionBase
{
    [SerializeField]
    protected float duration;

    private void Awake()
    {
        Destroy(gameObject, duration);
    }
}
