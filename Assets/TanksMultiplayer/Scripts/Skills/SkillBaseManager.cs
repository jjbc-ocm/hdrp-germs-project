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

    protected Player owner;

    protected Player autoTarget;

    public int Damage { get => damage; }

    public Player Owner { get => owner; }

    public Player AutoTarget { get => autoTarget; }

    void Awake()
    {
        Destroy(gameObject, lifeSpan);
    }

    public void Initialize(Player owner, Player autoTarget = null)
    {
        this.owner = owner;

        this.autoTarget = autoTarget;

        OnInitialize();
    }

    protected bool IsHit(Player origin, Player target)
    {
        if (target == owner || target == null) return false;

        else if (origin.photonView.GetTeam() == target.photonView.GetTeam()) return false;

        return true;
    }

    protected abstract void OnInitialize();
}
