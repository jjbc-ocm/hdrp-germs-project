using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public abstract class SkillBaseManager : MonoBehaviour
{
    [SerializeField]
    protected SkillData data;

    [SerializeField]
    protected int damage;

    [SerializeField]
    protected float lifeSpan;

    protected ActorManager owner;

    protected ActorManager autoTarget;

    public int Damage { get => damage; }

    public ActorManager Owner { get => owner; }

    public ActorManager AutoTarget { get => autoTarget; }

    void Awake()
    {
        Destroy(gameObject, lifeSpan);
    }

    public void Initialize(ActorManager owner, ActorManager autoTarget = null)
    {
        this.owner = owner;

        this.autoTarget = autoTarget;

        OnInitialize();
    }

    protected bool IsHit(ActorManager origin, ActorManager target)
    {
        if (target == owner || target == null) return false;

        if ((origin.IsMonster && !target.IsMonster) || (!origin.IsMonster && target.IsMonster)) return true;

        else if (origin.photonView.GetTeam() == target.photonView.GetTeam()) return false;

        return true;
    }

    protected abstract void OnInitialize();
}
