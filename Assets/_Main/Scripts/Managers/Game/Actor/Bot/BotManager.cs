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

        threads = new DecisionThreadInfo[4];

        threads[0] = new MoveThreadInfo();

        threads[1] = new AttackThreadInfo();

        threads[2] = new SkillThreadInfo();

        threads[3] = new ShopThreadInfo();

        foreach (var thread in threads)
        {
            thread.Initialize(player, agent, maxStatsInfo);
        }

        StartCoroutine(YieldDecisionMaking());
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

        var entities = GameManager.Instance.Entities;//FindObjectsOfType<GameEntityManager>();

        while (true)
        {
            yield return new WaitForSeconds(0.25f);

            if (player.Stat.Health <= 0) continue;

            foreach (var thread in threads)
            {
                thread.DecisionMaking(entities);

                yield return new WaitForSeconds(0.25f);
            }
        }
    }

    #endregion
}