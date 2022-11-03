using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public abstract class SkillBaseManager : MonoBehaviour
{
    [SerializeField]
    private int damage;

    [SerializeField]
    protected float lifeSpan;

    protected Rigidbody rigidBody;

    protected Player owner;

    public int Damage { get => damage; }

    public Player Owner { get => owner; }

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();

        Destroy(gameObject, lifeSpan);
    }

    public void Initialize(Player owner)
    {
        this.owner = owner;

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
