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

    private float GetWeightToApproachTarget(GameEntityManager target)
    {
        if (target is PlayerManager)
        {
            var targetPlayer = target as PlayerManager;

            /* Prioritize enemy carrying a chest */
            var targetChest = targetPlayer.Stat.HasChest && player.GetTeam() != targetPlayer.GetTeam() ? 1f : 0f;

            /* Prioritize enemy with less health */
            var targetHealthRatio = 1f - (targetPlayer.Stat.Health / (float)targetPlayer.Stat.MaxHealth());

            /* Derioritize enemy if player have less health */
            var selfHealthRatio = player.Stat.Health / (float)player.Stat.MaxHealth();

            /* Prioritize nearby enemies */
            var playerTargetDistance = Vector3.Distance(player.transform.position, targetPlayer.transform.position);

            var targetDistance = Mathf.Max(0f, 1f - (playerTargetDistance / SOManager.Instance.Constants.FogOrWarDistance));

            /* Prioritize enemies generally */
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

            /* Deprioritize monster when carrying a chest */
            var selfChest = !player.Stat.HasChest ? 1f : 0f;

            /* Prioritize monster with less health */
            var targetHealthRatio = 1f - (targetMonster.m_health.m_currentHealth / targetMonster.m_health.m_maxHealth);

            /* Deprioritize monster if player have less health */
            var selfHealthRatio = player.Stat.Health / (float)player.Stat.MaxHealth();

            /* Prioritize nearby monster */
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

            /* Deprioritize chest if player have less health */
            var selfHealthRatio = player.Stat.Health / (float)player.Stat.MaxHealth();

            return selfHealthRatio * personality.GetWeightMoveToChestPriority("selfHealthRatio");
        }

        if (target is Collectible && target is not CollectibleTeam)
        {
            var targetCollectible = target as Collectible;

            /* Prioritize nearby enemies */
            var playerTargetDistance = Vector3.Distance(player.transform.position, targetCollectible.transform.position);

            var targetDistance = Mathf.Max(0f, 1f - (playerTargetDistance / SOManager.Instance.Constants.FogOrWarDistance));

            return targetDistance * personality.GetWeightMoveToCollectiblePriority("targetDistance");
        }

        if (target is BaseManager)
        {
            var targetBase = target as BaseManager;

            /* Prioritize returning to base if player has the chest */
            var selfChest = player.Stat.HasChest && targetBase.Team == player.GetTeam() ? 1f : 0f;

            return selfChest * personality.GetWeightMoveToBasePriority("selfChest");
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
}
