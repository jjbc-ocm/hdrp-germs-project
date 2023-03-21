using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TanksMP;
using UnityEngine;
using UnityEngine.AI;

public abstract class DecisionThreadInfo
{
    protected PersonalityInfo personality;

    protected Player player;

    protected NavMeshAgent agent;

    protected ItemStatValuesInfo maxItemStatValues;

    public void Initialize(Player player, NavMeshAgent agent, ItemStatValuesInfo maxItemStatValues)
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
            //Debug.Log("[BOT: " + player.gameObject.name + "] " + finalDecision.Key);

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

    #region DECISION TO ENTITY

    
    private DecisionNodeInfo AttackDecision()
    {
        return new DecisionNodeInfo
        {
            Key = "Attack: True",

            Weight = 0f,

            Decision = () => player.Input.OnAttack(true)
        };
    }

    private DecisionNodeInfo AttackStopDecision()
    {
        return new DecisionNodeInfo
        {
            Key = "Attack: False",

            Weight = 0f,

            Decision = () => player.Input.OnAttack(false)
        };
    }

    

    #endregion

    #region DECISION TO ITEM

    

    #endregion
}
