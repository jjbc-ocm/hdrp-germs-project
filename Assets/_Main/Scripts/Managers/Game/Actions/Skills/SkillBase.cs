using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : ActionBase
{
    [SerializeField]
    protected float duration;

    /*private void Awake()
    {
        Destroy(gameObject, duration);
    }*/

    public override void OnGet()
    {
        StartCoroutine(YieldDespawn());
    }

    public override void OnRelease()
    {

    }

    private IEnumerator YieldDespawn()
    {
        yield return new WaitForSeconds(duration);

        PoolManager.Instance.Release(this);
    }
}
