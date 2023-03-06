using System;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class AttackThreadInfo : DecisionThreadInfo
{
    private bool isAttackFinalized;

    public override DecisionNodeInfo GetFinalDecision(List<GameEntityManager> entities)
    {
        DecisionNodeInfo currentDecision = null;

        isAttackFinalized = false;

        foreach (var entity in entities)
        {
            if (entity.IsVisibleRelativeTo(player.transform))
            {
                if (entity is Player && (entity as Player).GetTeam() != player.GetTeam())
                {
                    currentDecision = GetBetterDecision(currentDecision, AttackDecision(entity));
                }

                if (entity is GPMonsterBase && !(entity as GPMonsterBase).m_health.m_isDead)
                {
                    currentDecision = GetBetterDecision(currentDecision, AttackDecision(entity));
                }

                if (isAttackFinalized) break;
            }
        }

        return currentDecision;
    }

    private DecisionNodeInfo AttackDecision(GameEntityManager entity)
    {
        var isInfront = Vector3.Dot(player.transform.forward, entity.transform.position - player.transform.position) > 0.75f;
        // TODO: 0.75 should actually be a personality thing

        if (isInfront) isAttackFinalized = true;
        
        return new DecisionNodeInfo
        {
            Key = "Attack: TRUE",

            Weight = 1,

            Decision = () =>
            {
                player.Input.OnAttack(isInfront);
            }
        };
    }
}
