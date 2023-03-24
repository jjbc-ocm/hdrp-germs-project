using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : ActionBase
{
    [SerializeField]
    protected float duration;

    protected override void OnGet()
    {
        StartCoroutine(YieldDespawn());
    }

    protected override void OnRelease()
    {

    }

    private IEnumerator YieldDespawn()
    {
        yield return new WaitForSeconds(duration);

        PoolManager.Instance.Release(this);
    }
}
