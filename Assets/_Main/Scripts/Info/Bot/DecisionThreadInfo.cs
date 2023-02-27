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

    //private DecisionType type;

    protected ItemStatValuesInfo maxItemStatValues;

    /*public DecisionThreadInfo() { }

    public DecisionThreadInfo(Player player, NavMeshAgent agent, ItemStatValuesInfo maxItemStatValues)
    {
        personality = player.Data.Personality;

        this.player = player;

        this.agent = agent;

        //this.type = type;

        this.maxItemStatValues = maxItemStatValues;
    }*/

    public void Initialize(Player player, NavMeshAgent agent, ItemStatValuesInfo maxItemStatValues)
    {
        personality = player.Data.Personality;

        this.player = player;

        this.agent = agent;

        this.maxItemStatValues = maxItemStatValues;
    }

    public void DecisionMaking()
    {
        var finalDecision = GetFinalDecision();

        if (finalDecision != null)
        {
            Debug.Log("[BOT: " + player.gameObject.name + "] " + finalDecision.Key);

            finalDecision.Decision.Invoke();
        }
    }

    /* Everytime this method is called, it executes the best decision it can make.
     * This process might be CPU heavy since it is a series of loops. */
    /*public void DecisionMaking_Old()
    {
        DecisionNodeInfo currentDecision = null;

        *//* Find best decision this player can do to all entities in-game *//*
        if (type != DecisionType.Shop)
        {
            var entities = Object.FindObjectsOfType<GameEntityManager>();

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
        }
        
        *//* Find best possible decision this player can do in terms of buying items *//*
        else
        {
            foreach (var item in ShopManager.Instance.Data)
            {
                if (!ShopManager.Instance.CanBuy(player, item)) continue;

                currentDecision = GetBetterDecision(currentDecision, GetDecisionTo(item));
            }
        }

        if (currentDecision != null)
        {
            Debug.Log("[BOT: " + player.gameObject.name + "] " + currentDecision.Key);

            *//* Only one decision must be executed *//*
            currentDecision.Decision.Invoke();
        }
    }*/

    public abstract DecisionNodeInfo GetFinalDecision();

    protected DecisionNodeInfo GetBetterDecision(DecisionNodeInfo a, DecisionNodeInfo b)
    {
        if (a == null) return b;
        if (b == null) return a;

        return a.Weight > b.Weight ? a : b;
    }

    #region DECISION TO ENTITY

    /*private DecisionNodeInfo GetDecisionTo(GameEntityManager entity)
    {
        *//*if (type == DecisionType.Action)
        {*//*
            DecisionNodeInfo currentDecision = null;

            currentDecision = GetBetterDecision(currentDecision, AttackDecision());

            currentDecision = GetBetterDecision(currentDecision, AttackStopDecision());

            currentDecision = GetBetterDecision(currentDecision, AimDecision());

            *//*if (Vector3.Distance(player.transform.position, entity.transform.position) <= player.Skill.Range)
            {
                currentDecision = GetBetterDecision(currentDecision, AimStopDecision(entity));
            }
            else
            {
                currentDecision = GetBetterDecision(currentDecision, AimCancelDecision());
            }*//*

            return currentDecision;
        //}

        *//*if (type == DecisionType.Movement)
        {
            return new DecisionNodeInfo
            {
                Key = "Move to: " + entity.name,

                Weight = GetWeightToApproachTarget(entity),

                Decision = () => ApproachTargetDecision(entity.transform)
            };
        }*//*

        //return null;
    }*/

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

    /*private DecisionNodeInfo AimDecision()
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

    private DecisionNodeInfo AimCancelDecision()
    {
        return new DecisionNodeInfo
        {
            Key = "Skill Aim: False",

            Weight = 0.75f,

            Decision = () => player.Input.OnAimCancel(true)
        };
    }*/

    /*private void ApproachTargetDecision(Transform target)
    {
        var targetDestination = target.position;

        *//* Finally set a destination to create a path *//*
        agent.SetDestination(targetDestination);

        *//* Make the bot move along the path using the similar controls player have *//*
        if (agent.path.corners.Length > 1)
        {
            var targetPosition = agent.path.corners[1];

            var dotVertical = Vector3.Dot(player.transform.forward, (targetPosition - player.transform.position).normalized);

            var dotHorizontal = Vector3.Dot(player.transform.right, (targetPosition - player.transform.position).normalized);

            *//* Keep distance to the target *//*
            if (target.TryGetComponent(out ActorManager _) &&
                Vector3.Distance(player.transform.position, targetDestination) <= 25f) // TODO: range should actually be different based on role (melee, ranger, etc)
            {
                player.Input.OnMove(new Vector2(dotHorizontal, Mathf.Min(0, dotVertical)));
            }

            *//* Otherwise, move normally *//*
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

            *//* Prioritize enemy carrying a chest *//*
            var targetChest = targetPlayer.HasChest() && player.GetTeam() != targetPlayer.GetTeam() ? 1f : 0f;

            *//* Prioritize enemy with less health *//*
            var targetHealthRatio = 1f - (targetPlayer.Stat.Health / (float)targetPlayer.Stat.MaxHealth());

            *//* Derioritize enemy if player have less health *//*
            var selfHealthRatio = player.Stat.Health / (float)player.Stat.MaxHealth();

            *//* Prioritize nearby enemies *//*
            var targetDistance = Mathf.Max(0f, 1f - (Vector3.Distance(player.transform.position, targetPlayer.transform.position) / Constants.FOG_OF_WAR_DISTANCE));

            *//* Prioritize enemies generally *//*
            // TODO: this is a problem because what if my action is to cast a support skill
            // TODO: right now, player will not approach their ally
            var weightTeam = player.GetTeam() != targetPlayer.GetTeam() ? 1f : 0f;

            return
                targetChest * personality.GetWeightMoveToPlayerPriority("targetChest") +
                targetHealthRatio * personality.GetWeightMoveToPlayerPriority("targetHealthRatio") +
                selfHealthRatio * personality.GetWeightMoveToPlayerPriority("selfHealthRatio") +
                targetDistance * personality.GetWeightMoveToPlayerPriority("targetDistance") +
                weightTeam * 0.2f;
        }

        if (target is GPMonsterBase)
        {
            var targetMonster = target as GPMonsterBase;

            *//* Deprioritize monster when carrying a chest *//*
            var selfChest = !player.HasChest() ? 1f : 0f;

            *//* Prioritize monster with less health *//*
            var targetHealthRatio = 1f - (targetMonster.m_health.m_currentHealth / targetMonster.m_health.m_maxHealth);

            *//* Deprioritize monster if player have less health *//*
            var selfHealthRatio = player.Stat.Health / (float)player.Stat.MaxHealth();

            *//* Prioritize nearby monster *//*
            var targetDistance = Mathf.Max(0f, 1f - (Vector3.Distance(player.transform.position, targetMonster.transform.position) / 1000f));

            return
                selfChest * personality.GetWeightMoveToMonsterPriority("selfChest") +
                targetHealthRatio * personality.GetWeightMoveToMonsterPriority("targetHealthRatio") +
                selfHealthRatio * personality.GetWeightMoveToMonsterPriority("selfHealthRatio") +
                targetDistance * personality.GetWeightMoveToMonsterPriority("targetDistance");
        }

        if (target is CollectibleTeam)
        {
            var targetChest = target as CollectibleTeam;

            *//* Deprioritize chest if player have less health *//*
            var selfHealthRatio = player.Stat.Health / (float)player.Stat.MaxHealth();

            return selfHealthRatio * personality.GetWeightMoveToChestPriority("selfHealthRatio");
        }

        if (target is Collectible && target is not CollectibleTeam)
        {
            var targetCollectible = target as Collectible;

            *//* Prioritize nearby enemies *//*
            var targetDistance = Mathf.Max(0f, 1f - (Vector3.Distance(player.transform.position, targetCollectible.transform.position) / Constants.FOG_OF_WAR_DISTANCE));

            return targetDistance * personality.GetWeightMoveToCollectiblePriority("targetDistance");
        }

        if (target is CollectibleZone)
        {
            var targetZone = target as CollectibleZone;

            *//* Prioritize returning to base if player has the chest *//*
            var selfChest = player.HasChest() && targetZone.Team == player.GetTeam() ? 1f : 0f;

            return selfChest * personality.GetWeightMoveToBasePriority("selfChest");
        }

        return 0;
    }*/

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
        var healthRatio = item.StatModifier.BuffMaxHealth / maxItemStatValues.Health;

        var manaRatio = item.StatModifier.BuffMaxMana / maxItemStatValues.Mana;

        var attackDamageRatio = item.StatModifier.BuffAttackDamage / maxItemStatValues.AttackDamage;

        var abilityPowerRatio = item.StatModifier.BuffAbilityPower / maxItemStatValues.AbilityPower;

        var armorRatio = item.StatModifier.BuffArmor / maxItemStatValues.Armor;

        var resistRatio = item.StatModifier.BuffResist / maxItemStatValues.Resist;

        var attackSpeedRatio = item.StatModifier.BuffAttackSpeed / maxItemStatValues.AttackSpeed;

        var moveSpeedRatio = item.StatModifier.BuffMoveSpeed / maxItemStatValues.MoveSpeed;

        var lifeStealRatio = item.StatModifier.LifeSteal / maxItemStatValues.LifeSteal;

        var cooldownRatio = item.StatModifier.BuffCooldown / maxItemStatValues.Cooldown;

        var invisibilityRatio = item.StatModifier.IsInvisible ? 1f : 0f;

        var costRatio = 1 - item.CostBuy / maxItemStatValues.Cost;


        return
            healthRatio * personality.GetWeightBuyItem("healthRatio") +
            manaRatio * personality.GetWeightBuyItem("manaRatio") +
            attackDamageRatio * personality.GetWeightBuyItem("attackDamageRatio") +
            abilityPowerRatio * personality.GetWeightBuyItem("abilityPowerRatio") +
            armorRatio * personality.GetWeightBuyItem("armorRatio") +
            resistRatio * personality.GetWeightBuyItem("resistRatio") +
            attackSpeedRatio * personality.GetWeightBuyItem("attackSpeedRatio") +
            moveSpeedRatio * personality.GetWeightBuyItem("moveSpeedRatio") +
            lifeStealRatio * personality.GetWeightBuyItem("lifeStealRatio") +
            cooldownRatio * personality.GetWeightBuyItem("cooldownRatio") +
            invisibilityRatio * personality.GetWeightBuyItem("invisibilityRatio") +
            costRatio * personality.GetWeightBuyItem("costRatio");
    }

    

    #endregion
}
