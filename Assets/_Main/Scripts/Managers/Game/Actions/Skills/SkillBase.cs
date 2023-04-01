using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : ActionBase
{
    [SerializeField]
    protected float duration;

    private void Awake()
    {
        StartCoroutine(YieldDespawn());
    }

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

        Destroy(gameObject);
        //PoolManager.Instance.Release(this);
    }
}
