using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;
using UnityEngine.AI;

public class MoveThreadInfo : DecisionThreadInfo
{
    public override DecisionNodeInfo GetFinalDecision(List<GameEntityManager> entities)
    {
        DecisionNodeInfo currentDecision = null;

        foreach (var entity in entities)
        {
            if (entity.IsVisibleRelativeTo(player.transform))
            {
                if (entity is PlayerManager && entity != player)
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

            if (entity is BaseManager)
            {
                currentDecision = GetBetterDecision(currentDecision, GetDecisionTo(entity));
            }
        }

        return currentDecision;
    }

    private DecisionNodeInfo GetDecisionTo(GameEntityManager entity)
    {
        return new DecisionNodeInfo
        {
            Key = "Move to: " + entity.name,

            Weight = GetWeightToApproachTarget(entity),

            Decision = () => ApproachTargetDecision(entity.transform)
        };
    }
    
    private float GetFinalWeight(List<MovePriorityInfo> priorities, GameEntityManager target)
    {
        var weight = 0f;

        foreach (var prio in priorities)
        {
            var other = prio.Target == BotTargetType.Target ? target : null;

            weight += GetMoveDecisionWeight(other, prio.Property, prio.IsInverse) * prio.Weight;
        }

        return weight;
    }

    private float GetWeightToApproachTarget(GameEntityManager target)
    {
        // Decisions if target is a player
        if (target is PlayerManager)
        {
            var targetPlayer = target as PlayerManager;

            // Decisions if target player is ally
            if (player.GetTeam() == targetPlayer.GetTeam())
            {
                return GetFinalWeight(personality.MoveToAllyPlayerConditions, targetPlayer);
            }

            // Decisions if target player if enemy
            else
            {
                return GetFinalWeight(personality.MoveToEnemyPlayerConditions, targetPlayer);
            }
        }

        // Decision if target is monster
        if (target is GPMonsterBase)
        {
            var targetMonster = target as GPMonsterBase;

            return GetFinalWeight(personality.MoveToMonsterConditions, targetMonster);
        }

        // Decision if target is key
        if (target is KeyCollectible)
        {
            var targetKey = target as KeyCollectible;

            return GetFinalWeight(personality.MoveToKeyConditions, targetKey);
        }
        
        // Decision if target is chest
        if (target is ChestCollectible)
        {
            var targetChest = target as ChestCollectible;

            return GetFinalWeight(personality.MoveToChestConditions, targetChest);
        }
        
        // Decision if target is normal item
        if (target is Collectible && target is not ChestCollectible && target is not KeyCollectible)
        {
            var targetCollectible = target as Collectible;

            return GetFinalWeight(personality.MoveToCollectibleConditions, targetCollectible);
        }

        // Decisiob if target is base
        if (target is BaseManager)
        {
            var targetBase = target as BaseManager;

            // Decisions if target base is ally
            if (player.GetTeam() == targetBase.Team)
            {
                return GetFinalWeight(personality.MoveToAllyBaseConditions, targetBase);
            }

            // Decisions if target base if enemy
            else
            {
                return GetFinalWeight(personality.MoveToEnemyBaseConditions, targetBase);
            }
        }

        return 0;
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

            // Keep distance to the target if it is a target
            if (target.TryGetComponent(out ActorManager _) &&
                Vector3.Distance(player.transform.position, targetDestination) <= 25f) // TODO: range should actually be different based on role (melee, ranger, etc)
            {
                player.Input.OnMove(new Vector2(dotHorizontal, Mathf.Min(0, dotVertical)));
            }

            // Otherwise, move normally
            else
            {
                player.Input.OnMove(new Vector2(dotHorizontal, dotVertical));
            }

        }
    }
}
