using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BotManager : MonoBehaviour
{
    #region Components

    private NavMeshAgent agent;

    private Player player;

    #endregion

    private DecisionThreadInfo[] threads;

    private BotInfo info;

    public BotInfo Info { get => info; }

    #region Unity

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        player = GetComponent<Player>();

        
    }

    private void Start()
    {
        var maxStatsInfo = GetMaxItemStatValues();

        threads = new DecisionThreadInfo[2];

        threads[0] = new MoveThreadInfo();

        threads[1] = new SkillThreadInfo();

        //threads[0] = new DecisionThreadInfo(player, agent, DecisionType.Action, maxStatsInfo);

        //threads[1] = new DecisionThreadInfo(player, agent, DecisionType.Movement, maxStatsInfo);

        //threads[2] = new DecisionThreadInfo(player, agent, DecisionType.Shop, maxStatsInfo);

        foreach (var thread in threads)
        {
            thread.Initialize(player, agent, maxStatsInfo);
        }

        StartCoroutine(YieldDecisionMaking());
    }

    private void Update()
    {
        agent.stoppingDistance = 5f; // TODO: do not hard-code

        //agent.speed = player.Stat.MoveSpeed;

        //agent.angularSpeed = player.Stat.MoveSpeed;

        for (var i = 0; i < agent.path.corners.Length - 1; i++)
        {
            var a = agent.path.corners[i];
            var b = agent.path.corners[i + 1];

            Debug.DrawLine(a, b, player.GetTeam() == 0 ? Color.red : Color.blue);
        }
    }

    #endregion

    #region Public

    public void Initialize(BotInfo info)
    {
        this.info = info;
    }

    public Ray GetRay()
    {
        var origin = transform.position + transform.up * 5 - transform.forward * 5; // TODO: test bukas umaga

        return new Ray(origin, (player.Input.BotLook - origin).normalized);
    }

    #endregion

    #region Private

    private ItemStatValuesInfo GetMaxItemStatValues()
    {
        var itemStatValues = new ItemStatValuesInfo();

        foreach (var item in ShopManager.Instance.Data)
        {
            if (item.StatModifier.BuffMaxHealth > itemStatValues.Health)
                itemStatValues.Health = item.StatModifier.BuffMaxHealth;

            if (item.StatModifier.BuffMaxMana > itemStatValues.Mana)
                itemStatValues.Mana = item.StatModifier.BuffMaxMana;

            if (item.StatModifier.BuffAttackDamage > itemStatValues.AttackDamage)
                itemStatValues.AttackDamage = item.StatModifier.BuffAttackDamage;

            if (item.StatModifier.BuffAbilityPower > itemStatValues.AbilityPower)
                itemStatValues.AbilityPower = item.StatModifier.BuffAbilityPower;

            if (item.StatModifier.BuffArmor > itemStatValues.Armor)
                itemStatValues.Armor = item.StatModifier.BuffArmor;

            if (item.StatModifier.BuffResist > itemStatValues.Resist)
                itemStatValues.Resist = item.StatModifier.BuffResist;

            if (item.StatModifier.BuffAttackSpeed > itemStatValues.AttackSpeed)
                itemStatValues.AttackSpeed = item.StatModifier.BuffAttackSpeed;

            if (item.StatModifier.BuffMoveSpeed > itemStatValues.MoveSpeed)
                itemStatValues.MoveSpeed = item.StatModifier.BuffMoveSpeed;

            if (item.StatModifier.LifeSteal > itemStatValues.LifeSteal)
                itemStatValues.LifeSteal = item.StatModifier.LifeSteal;

            if (item.StatModifier.BuffCooldown > itemStatValues.Cooldown)
                itemStatValues.Cooldown = item.StatModifier.BuffCooldown;

            if (item.CostBuy > itemStatValues.Cost)
                itemStatValues.Cost = item.CostBuy;
        }

        return itemStatValues;
    }

    private IEnumerator YieldDecisionMaking()
    {
        yield return new WaitForSeconds(Random.value);

        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (player.Stat.Health <= 0) continue;

            foreach (var thread in threads)
            {
                thread.DecisionMaking();
            }
        }
    }

    #endregion
}

/*public enum DecisionType
{
    Action,
    Movement,
    Shop
}*/