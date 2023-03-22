using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public abstract class ActionBase : MonoBehaviour
{
    [SerializeField]
    protected SkillData data;

    [SerializeField]
    protected int damage;

    protected ActorManager owner;

    protected Vector3 targetPosition;

    protected ActorManager targetActor;

    public int Damage { get => damage; }

    public ActorManager Owner { get => owner; }

    #region Public

    public void Initialize(ActorManager owner, Vector3 targetPosition, ActorManager targetActor = null)
    {
        this.owner = owner;

        this.targetPosition = targetPosition;

        this.targetActor = targetActor;

        OnInitialize();
    }

    #endregion

    #region Protected

    protected void ApplyEffect(ActorManager user, ActorManager target)
    {
        target.photonView.RPC("RpcDamageHealth", RpcTarget.All, damage, user.photonView.ViewID);

        if (user.TryGetComponent(out PlayerManager playerUser))//(owner is Player)
        {
            var lifeSteal = -Mathf.Max(1, Mathf.RoundToInt(damage * playerUser.Inventory.StatModifier.LifeSteal));

            playerUser.photonView.RPC("RpcDamageHealth", RpcTarget.All, lifeSteal, 0);
        }
    }

    protected bool IsHit(ActorManager origin, ActorManager target) // TODO: this need to be dynamic, because what if the skill is suppose to be targeting the ally instead of the enemy
    {
        if (target == owner || target == null) return false;

        if ((origin.IsMonster && !target.IsMonster) || (!origin.IsMonster && target.IsMonster)) return true;

        else if (origin.GetTeam() == target.GetTeam()) return false;

        return true;
    }

    #endregion

    #region Abstract

    protected abstract void OnInitialize();

    public abstract void OnGet();

    public abstract void OnRelease();

    #endregion
}
