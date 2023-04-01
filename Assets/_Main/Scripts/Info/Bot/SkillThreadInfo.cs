using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class SkillThreadInfo : DecisionThreadInfo
{
    private GameEntityManager aimTarget;

    public override DecisionNodeInfo GetFinalDecision(List<GameEntityManager> entities)
    {
        DecisionNodeInfo currentDecision = null;

        foreach (var entity in entities)
        {
            if (Vector3.Distance(player.transform.position, entity.transform.position) <= player.Skill.Range)
            {
                if (entity is PlayerManager && (entity as PlayerManager).GetTeam() != player.GetTeam()) // TODO: this will be a problem if skill was intended to use to allies
                {
                    currentDecision = GetBetterDecision(currentDecision, AimCancelDecision());

                    currentDecision = GetBetterDecision(currentDecision, AimReleaseDecision());

                    currentDecision = GetBetterDecision(currentDecision, AimDecision(entity));
                }

                if (entity is GPMonsterBase && !(entity as GPMonsterBase).m_health.m_isDead)
                {
                    currentDecision = GetBetterDecision(currentDecision, AimCancelDecision());

                    currentDecision = GetBetterDecision(currentDecision, AimReleaseDecision());

                    currentDecision = GetBetterDecision(currentDecision, AimDecision(entity));
                }
            }
        }

        return currentDecision;
    }

    private DecisionNodeInfo AimDecision(GameEntityManager entity)
    {
        return new DecisionNodeInfo
        {
            Key = "Skill Aim: " + entity.name,

            Weight = 0.8f,

            Decision = () =>
            {
                player.Input.OnAim(true);

                aimTarget = entity;
            }
        };
    }

    private DecisionNodeInfo AimReleaseDecision()
    {
        return new DecisionNodeInfo
        {
            Key = "Skill Release Aim: " + aimTarget?.name ?? "NULL",

            Weight = aimTarget != null ? 0.9f : 0f,

            Decision = () =>
            {
                player.Input.OnLook(aimTarget.transform.position);

                player.Input.OnAim(false);

                aimTarget = null;
            }
        };
    }

    private DecisionNodeInfo AimCancelDecision()
    {
        return new DecisionNodeInfo
        {
            Key = "Skill Cancel Aim: " + aimTarget?.name ?? "NULL",

            Weight = aimTarget != null && !aimTarget.IsVisibleRelativeTo(player.transform) ? 1f : 0f, // TODO: should actually use the distance of skill instead of this

            Decision = () =>
            {
                player.Input.OnAimCancel(true);

                aimTarget = null;
            }
        };
    }
}
