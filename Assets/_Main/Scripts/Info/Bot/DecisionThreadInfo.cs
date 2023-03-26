using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TanksMP;
using UnityEngine;
using UnityEngine.AI;

public abstract class DecisionThreadInfo
{
    protected PersonalityInfo personality;

    protected PlayerManager player;

    protected NavMeshAgent agent;

    protected ItemStatValuesInfo maxItemStatValues;

    public void Initialize(PlayerManager player, NavMeshAgent agent, ItemStatValuesInfo maxItemStatValues)
    {
        personality = player.Data.Personality;

        this.player = player;

        this.agent = agent;

        this.maxItemStatValues = maxItemStatValues;
    }

    public void DecisionMaking(List<GameEntityManager> entities)
    {
        var finalDecision = GetFinalDecision(entities);

        if (finalDecision != null)
        {
            Debug.Log("[BOT: " + player.gameObject.name + "] " + finalDecision.Key + " | Weight: " + finalDecision.Weight);

            finalDecision.Decision.Invoke();
        }
    }

    

    public abstract DecisionNodeInfo GetFinalDecision(List<GameEntityManager> entities);

    protected DecisionNodeInfo GetBetterDecision(DecisionNodeInfo a, DecisionNodeInfo b)
    {
        if (a == null) return b;
        if (b == null) return a;

        return a.Weight > b.Weight ? a : b;
    }

    #region Decisions

    protected float GetDecisionWeight(GameEntityManager other, BotPropertyType propertyType, bool isInverse)
    {
        var target = other ?? player;

        // Note: It's value must always be between or equal 0 and 1
        var value = 0f;

        if (propertyType == BotPropertyType.Distance)
        {
            var distance = Vector3.Distance(player.transform.position, target.transform.position);

            value = Mathf.Min(1f, distance / SOManager.Instance.Constants.FogOrWarDistance);
        }

        else if (propertyType == BotPropertyType.DistanceNoFOV)
        {
            var distance = Vector3.Distance(player.transform.position, target.transform.position);

            value = Mathf.Min(1f, distance / 1000f);
        }

        else if (propertyType == BotPropertyType.Key)
        {
            if (target is PlayerManager)
            {
                value = (target as PlayerManager).Stat.HasKey ? 1f : 0f;
            }
        }

        else if (propertyType == BotPropertyType.Chest)
        {
            if (target is PlayerManager)
            {
                value = (target as PlayerManager).Stat.HasChest ? 1f : 0f;
            }
        }

        else if (propertyType == BotPropertyType.HealthRatio)
        {
            if (target is PlayerManager)
            {
                value = (target as PlayerManager).Stat.Health / (float)(target as PlayerManager).Stat.MaxHealth();
            }

            if (target is GPMonsterBase)
            {
                value = (target as GPMonsterBase).m_health.m_currentHealth / (target as GPMonsterBase).m_health.m_maxHealth;
            }
        }

        if (isInverse) value = 1f - value;

        return value;
    }

    #endregion
}


