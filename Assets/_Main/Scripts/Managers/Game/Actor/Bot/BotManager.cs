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

        threads = new DecisionThreadInfo[2];

        threads[0] = new DecisionThreadInfo(player, agent, DecisionType.Action);

        threads[1] = new DecisionThreadInfo(player, agent, DecisionType.Movement);
    }

    private void Start()
    {
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

    #endregion

    #region Private

    private IEnumerator YieldDecisionMaking()
    {
        yield return new WaitForSeconds(Random.value);

        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            foreach (var thread in threads)
            {
                thread.DecisionMaking();
            }
        }
    }

    #endregion
}

public class DecisionThreadInfo
{
    private Player player;

    private NavMeshAgent agent;

    private DecisionType type;

    public DecisionThreadInfo(Player player, NavMeshAgent agent, DecisionType type)
    {
        this.player = player;

        this.agent = agent;

        this.type = type;
    }

    public void DecisionMaking()
    {
        DecisionNodeInfo currentDecision = null;

        var entities = player.transform.GetEntityInRange(Constants.FOG_OF_WAR_DISTANCE);

        foreach (var entity in entities)
        {
            if (entity is Player && entity != player)
            {
                var newDecision = GetDecisionTo(entity);

                currentDecision = GetBetterDecision(currentDecision, newDecision);
            }

            if (entity is GPMonsterBase)
            {
                var targetMonster = entity as GPMonsterBase;


            }

            if (entity is Collectible || entity is CollectibleZone)
            {
                var newDecision = GetDecisionTo(entity);

                currentDecision = GetBetterDecision(currentDecision, newDecision);
            }
        }

        currentDecision.Decision.Invoke();
    }

    private DecisionNodeInfo GetBetterDecision(DecisionNodeInfo a, DecisionNodeInfo b)
    {
        if (a == null) return b;
        if (b == null) return a;

        return a.Weight > b.Weight ? a : b;
    }

    private DecisionNodeInfo GetDecisionTo(GameEntityManager entity)
    {
        if (type == DecisionType.Action)
        {
            DecisionNodeInfo currentDecision = null;

            currentDecision = GetBetterDecision(currentDecision, AttackDecision());

            currentDecision = GetBetterDecision(currentDecision, AttackStopDecision());

            currentDecision = GetBetterDecision(currentDecision, AimDecision());

            currentDecision = GetBetterDecision(currentDecision, AimStopDecision());

            return currentDecision;
        }

        if (type == DecisionType.Movement)
        {
            return new DecisionNodeInfo
            {
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
            Weight = 1f,

            Decision = () => player.Input.OnAttack(true)
        };
    }

    private DecisionNodeInfo AttackStopDecision()
    {
        return new DecisionNodeInfo
        {
            Weight = 0f,

            Decision = () => player.Input.OnAttack(false)
        };
    }

    private DecisionNodeInfo AimDecision()
    {
        return new DecisionNodeInfo
        {
            Weight = 1f,

            Decision = () => player.Input.OnAim(true)
        };
    }

    private DecisionNodeInfo AimStopDecision()
    {
        return new DecisionNodeInfo
        {
            Weight = player.Aim.IsAiming ? 1f : 0f, //player.Input.IsAim ? 1f : 0f,

            Decision = () =>
            {
                player.Input.OnLook(Vector2.zero); // TODO: should have diff implem from bot

                player.Input.OnAim(false);
            }
        };
    }

    private void ApproachTargetDecision(Transform target)
    {
        var targetDestination = target.position;

        // TODO: one problem, player must rotate towards the targetPlayer, not just the final destination
        // TDOO: it means even when they reached the stopping distance, they must continue to rotate until they face the enemy

        /* Make adjustment to target destination to keep distance to the target */
        /*if (Vector3.Distance(player.transform.position, targetDestination) < Constants.FOG_OF_WAR_DISTANCE / 2f)
        {
            targetDestination = (player.transform.position - targetDestination).normalized * -Constants.FOG_OF_WAR_DISTANCE;
        }*/

        /* Finally set a destination to create a path */
        agent.SetDestination(targetDestination);

        /* Make the bot move along the path using the similar controls player have */
        if (agent.path.corners.Length > 1)
        {
            var targetPosition = agent.path.corners[1];

            var dotVertical = Vector3.Dot(player.transform.forward, (targetPosition - player.transform.position).normalized);

            var dotHorizontal = Vector3.Dot(player.transform.right, (targetPosition - player.transform.position).normalized);

            player.Input.OnMove(new Vector2(dotHorizontal, dotVertical));
        }
    }
    
    private float GetWeightToApproachTarget(GameEntityManager target)
    {
        if (target is Player)
        {
            var targetPlayer = target as Player;

            /* Has Chest and Enemy - 100%
             * Other Reason - 0% */
            var weightChest = targetPlayer.HasChest() && player.GetTeam() != targetPlayer.GetTeam() ? 1f : 0f;

            /* 0% Health - 100% Weight
             * 100% Health - 0% Weight */
            var weightHealthRatio = 1f - (targetPlayer.Stat.Health / (float)targetPlayer.Stat.MaxHealth);

            /* 0 Meters - 100% Weight
             * >= 150 Meters - 0% Weight */
            var weightDistance = Mathf.Max(0f, 1f - (Vector3.Distance(player.transform.position, targetPlayer.transform.position) / 150f));

            /* Different Team - 100% Weight
             * Same Team - 0% Weight */
            // TODO: this is a problem because what if my action is to cast a support skill
            var weightTeam = player.GetTeam() != targetPlayer.GetTeam() ? 1f : 0f;

            return 
                weightChest * 0.25f + 
                weightHealthRatio * 0.25f + 
                weightTeam * 0.25f + 
                weightDistance * 0.25f;
        }

        if (target is CollectibleTeam)
        {
            var weightChest = target is CollectibleTeam ? 1f : 0f;

            return weightChest * 1f;
        }

        if (target is CollectibleZone)
        {
            var targetZone = target as CollectibleZone;

            /* Has Chest and Target is Mine - 100% Weight
             * Other Reason - 0% Weight */
            var weightChest = player.HasChest() && targetZone.Team == player.GetTeam() ? 1f : 0f;

            return weightChest * 1f;
        }

        return 0;
    }
}

public enum DecisionType
{
    Action,
    Movement
}