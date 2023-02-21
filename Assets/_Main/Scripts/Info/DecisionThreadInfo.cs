using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TanksMP;
using UnityEngine;
using UnityEngine.AI;

public class DecisionThreadInfo
{
    private Player player;

    private NavMeshAgent agent;

    private DecisionType type;

    private ItemStatValuesInfo maxItemStatValues;

    public DecisionThreadInfo(Player player, NavMeshAgent agent, DecisionType type, ItemStatValuesInfo maxItemStatValues)
    {
        this.player = player;

        this.agent = agent;

        this.type = type;

        this.maxItemStatValues = maxItemStatValues;
    }

    /* Everytime this method is called, it executes the best decision it can make.
     * This process might be CPU heavy since it is a series of loops. */
    public void DecisionMaking()
    {
        DecisionNodeInfo currentDecision = null;

        var entities = Object.FindObjectsOfType<GameEntityManager>();

        /* Find best decision this player can do to all entities in-game */
        foreach (var entity in entities)
        {
            if (entity.IsVisibleRelativeTo(player.transform))
            {
                if (entity is Player && entity != player)
                {
                    currentDecision = GetBetterDecision(currentDecision, GetDecisionTo(entity));
                }

                if (entity is Collectible)
                {
                    currentDecision = GetBetterDecision(currentDecision, GetDecisionTo(entity));
                }
            }

            if (entity is GPMonsterBase && !(entity as GPMonsterBase).m_health.m_isDead)
            {
                currentDecision = GetBetterDecision(currentDecision, GetDecisionTo(entity));
            }

            if (entity is CollectibleZone)
            {
                currentDecision = GetBetterDecision(currentDecision, GetDecisionTo(entity));
            }
        }

        /* Find best possible decision this player can do in terms of buying items */
        foreach (var item in ShopManager.Instance.Data)
        {
            if (!ShopManager.Instance.CanBuy(player, item)) continue;

            currentDecision = GetBetterDecision(currentDecision, GetDecisionTo(item));
        }

        Debug.Log("[BOT] " + currentDecision.Key);

        /* Only one decision must be executed */
        currentDecision.Decision.Invoke();
    }

    private DecisionNodeInfo GetBetterDecision(DecisionNodeInfo a, DecisionNodeInfo b)
    {
        if (a == null) return b;
        if (b == null) return a;

        return a.Weight > b.Weight ? a : b;
    }

    #region DECISION TO ENTITY

    private DecisionNodeInfo GetDecisionTo(GameEntityManager entity)
    {
        if (type == DecisionType.Action)
        {
            DecisionNodeInfo currentDecision = null;

            currentDecision = GetBetterDecision(currentDecision, AttackDecision());

            currentDecision = GetBetterDecision(currentDecision, AttackStopDecision());

            currentDecision = GetBetterDecision(currentDecision, AimDecision());

            if (Vector3.Distance(player.transform.position, entity.transform.position) <= player.Skill.Range)
            {
                currentDecision = GetBetterDecision(currentDecision, AimStopDecision(entity));
            }

            return currentDecision;
        }

        if (type == DecisionType.Movement)
        {
            return new DecisionNodeInfo
            {
                Key = "Move to: " + entity.name,

                Weight = GetWeightToApproachTarget(entity),

                Decision = () => ApproachTargetDecision(entity.transform)
            };
        }

        return null;
    }

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

    private DecisionNodeInfo AimDecision()
    {
        return new DecisionNodeInfo
        {
            Key = "Skill Aim: True",

            Weight = 0.5f,

            Decision = () => player.Input.OnAim(true)
        };
    }

    private DecisionNodeInfo AimStopDecision(GameEntityManager entity)
    {
        return new DecisionNodeInfo
        {
            Key = "Skill Aim: False\nTarget: " + entity.name,

            Weight = player.Aim.IsAiming ? 1f : 0f, //player.Input.IsAim ? 1f : 0f,

            Decision = () =>
            {
                player.Input.OnLook(entity.transform.position);

                player.Input.OnAim(false);
            }
        };
    }

    private void ApproachTargetDecision(Transform target)
    {
        var targetDestination = target.position;

        /* Finally set a destination to create a path */
        agent.SetDestination(targetDestination);

        /* Make the bot move along the path using the similar controls player have */
        if (agent.path.corners.Length > 1)
        {
            var targetPosition = agent.path.corners[1];

            var dotVertical = Vector3.Dot(player.transform.forward, (targetPosition - player.transform.position).normalized);

            var dotHorizontal = Vector3.Dot(player.transform.right, (targetPosition - player.transform.position).normalized);

            /* Keep distance to the target */
            if (target.TryGetComponent(out ActorManager _) &&
                Vector3.Distance(player.transform.position, targetDestination) <= 25f) // TODO: range should actually be different based on role (melee, ranger, etc)
            {
                player.Input.OnMove(new Vector2(dotHorizontal, Mathf.Min(0, dotVertical)));
            }

            /* Otherwise, move normally */
            else
            {
                player.Input.OnMove(new Vector2(dotHorizontal, dotVertical));
            }

        }
    }

    private float GetWeightToApproachTarget(GameEntityManager target)
    {
        if (target is Player)
        {
            var targetPlayer = target as Player;

            /* Prioritize enemy carrying a chest */
            var weightChest = targetPlayer.HasChest() && player.GetTeam() != targetPlayer.GetTeam() ? 1f : 0f;

            /* Prioritize enemy with less health */
            var weightEnemyHealthRatio = 1f - (targetPlayer.Stat.Health / (float)targetPlayer.Stat.MaxHealth());

            /* Derioritize enemy if player have less health */
            var weightSelfHealthRatio = player.Stat.Health / (float)player.Stat.MaxHealth();

            /* Prioritize nearby enemies */
            var weightDistance = Mathf.Max(0f, 1f - (Vector3.Distance(player.transform.position, targetPlayer.transform.position) / Constants.FOG_OF_WAR_DISTANCE));

            /* Prioritize enemies generally */
            // TODO: this is a problem because what if my action is to cast a support skill
            var weightTeam = player.GetTeam() != targetPlayer.GetTeam() ? 1f : 0f;

            return
                weightChest * 0.2f +
                weightEnemyHealthRatio * 0.2f +
                weightSelfHealthRatio * 0.2f +
                weightTeam * 0.2f +
                weightDistance * 0.2f;
        }

        if (target is GPMonsterBase)
        {
            var targetMonster = target as GPMonsterBase;

            /* Deprioritize monster when carrying a chest */
            var weightChest = !player.HasChest() ? 1f : 0f;

            /* Prioritize monster with less health */
            var weightMonsterHealthRatio = 1f - (targetMonster.m_health.m_currentHealth / targetMonster.m_health.m_maxHealth);

            /* Deprioritize monster if player have less health */
            var weightSelfHealthRatio = player.Stat.Health / (float)player.Stat.MaxHealth();

            /* Prioritize nearby monster */
            var weightDistance = Mathf.Max(0f, 1f - (Vector3.Distance(player.transform.position, targetMonster.transform.position) / 1000f));

            return
                weightChest * 0.25f +
                weightMonsterHealthRatio * 0.25f +
                weightSelfHealthRatio * 0.25f +
                weightDistance * 0.25f;
        }

        if (target is CollectibleTeam)
        {
            var targetChest = target as CollectibleTeam;

            /* Deprioritize chest if player have less health */
            var weightSelfHealthRatio = player.Stat.Health / (float)player.Stat.MaxHealth();

            return weightSelfHealthRatio * 1f;
        }

        if (target is CollectibleZone)
        {
            var targetZone = target as CollectibleZone;

            /* Prioritize returning to base if player has the chest */
            var weightChest = player.HasChest() && targetZone.Team == player.GetTeam() ? 1f : 0f;

            return weightChest * 1f;
        }

        return 0;
    }

    #endregion

    #region DECISION TO ITEM

    private DecisionNodeInfo GetDecisionTo(ItemData item)
    {
        return new DecisionNodeInfo
        {
            Key = "Buy Item : " + item.Name,

            Weight = GetWeightToItem(item),

            Decision = () =>
            {
                ShopManager.Instance.Buy(player, item);
            }
        };
    }

    private float GetWeightToItem(ItemData item)
    {
        var weightCost = item.CostBuy / maxItemStatValues.Cost;

        var parametersCount = 11f;

        var weightStatInc = 
            item.StatModifier.BuffMaxHealth / maxItemStatValues.Health 
            + item.StatModifier.BuffMaxMana / maxItemStatValues.Mana 
            + item.StatModifier.BuffAttackDamage / maxItemStatValues.AttackDamage 
            + item.StatModifier.BuffAbilityPower / maxItemStatValues.AbilityPower 
            + item.StatModifier.BuffArmor / maxItemStatValues.Armor 
            + item.StatModifier.BuffResist / maxItemStatValues.Resist 
            + item.StatModifier.BuffAttackSpeed / maxItemStatValues.AttackSpeed 
            + item.StatModifier.BuffMoveSpeed / maxItemStatValues.MoveSpeed 
            + item.StatModifier.LifeSteal / maxItemStatValues.LifeSteal 
            + item.StatModifier.BuffCooldown / maxItemStatValues.Cooldown 
            + (item.StatModifier.IsInvisible ? 1f : 0f)
            / parametersCount;

        var weightFurtherStatInc = 1f;

        var enemyAverageStats = GetAverageStats(Object.FindObjectsOfType<ActorManager>());

        var weightCounter = 1f;

        return
            weightCost * 0.25f +
            weightStatInc * 0.25f +
            weightFurtherStatInc * 0.25f +
            weightCounter * 0.25f;
    }

    private ItemStatValuesInfo GetAverageStats(ActorManager[] actors)
    {
        var enemies = actors.Where(i => i.GetTeam() != player.GetTeam());

        var itemStatValues = new ItemStatValuesInfo();

        /*
         * 
         * maxItemStatValues.Health 
            + item.StatModifier.BuffMaxMana / maxItemStatValues.Mana 
            + item.StatModifier.BuffAttackDamage / maxItemStatValues.AttackDamage 
            + item.StatModifier.BuffAbilityPower / maxItemStatValues.AbilityPower 
            + item.StatModifier.BuffArmor / maxItemStatValues.Armor 
            + item.StatModifier.BuffResist / maxItemStatValues.Resist 
            + item.StatModifier.BuffAttackSpeed / maxItemStatValues.AttackSpeed 
            + item.StatModifier.BuffMoveSpeed / maxItemStatValues.MoveSpeed 
            + item.StatModifier.LifeSteal / maxItemStatValues.LifeSteal 
            + item.StatModifier.BuffCooldown / maxItemStatValues.Cooldown 
         */

        var enemiesCount = (float)enemies.Count();

        foreach (var enemy in enemies)
        {
            itemStatValues.Health += enemy.Stat.MaxHealth() / enemiesCount;
            itemStatValues.Mana += enemy.Stat.MaxMana() / enemiesCount;
            itemStatValues.AttackDamage += enemy.Stat.AttackDamage() / enemiesCount;
            itemStatValues.AbilityPower += enemy.Stat.AbilityPower() / enemiesCount;
            itemStatValues.Armor += enemy.Stat.Armor() / enemiesCount;
            itemStatValues.Resist += enemy.Stat.Resist() / enemiesCount;
            itemStatValues.AttackSpeed += enemy.Stat.AttackSpeed() / enemiesCount;
            itemStatValues.MoveSpeed += enemy.Stat.MoveSpeed() / enemiesCount;

        }

        return itemStatValues;
    }

    #endregion
}
