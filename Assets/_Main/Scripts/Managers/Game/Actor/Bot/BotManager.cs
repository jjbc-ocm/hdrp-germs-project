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

    public Ray GetRay()
    {
        var origin = transform.position + transform.up * 5 - transform.forward * 5; // TODO: test bukas umaga

        return new Ray(origin, (player.Input.BotLook - origin).normalized);
    }

    #endregion

    #region Private

    private IEnumerator YieldDecisionMaking()
    {
        yield return new WaitForSeconds(Random.value);

        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (player.Stat.Health <= 0) continue;

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

        foreach (var item in ShopManager.Instance.Data)
        {
            if (!ShopManager.Instance.CanBuy(player, item)) continue;
            
            currentDecision = GetBetterDecision(currentDecision, GetDecisionTo(item));
        }

        Debug.Log("[BOT] " + currentDecision.Key);

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

    private DecisionNodeInfo GetDecisionTo(ItemData item)
    {
        return new DecisionNodeInfo
        {
            Key = "Buy Item : " + item.Name,
            
            Weight = 1f,

            Decision = () =>
            {
                ShopManager.Instance.Buy(player, item);
            }
        };
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
                /* This is necessary to make it look like aiming at the water below.
                 * Without it, bot cannot use skills that aim the water */
                var submergeOffset = player.Skill.Aim == AimType.Water ? Vector3.down : Vector3.zero;

                player.Input.OnLook(entity.transform.position + submergeOffset);

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
            var weightEnemyHealthRatio = 1f - (targetPlayer.Stat.Health / (float)targetPlayer.Stat.MaxHealth);

            /* Derioritize enemy if player have less health */
            var weightSelfHealthRatio = player.Stat.Health / (float)player.Stat.MaxHealth;

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
            var weightSelfHealthRatio = player.Stat.Health / (float)player.Stat.MaxHealth;

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
            var weightSelfHealthRatio = player.Stat.Health / (float)player.Stat.MaxHealth;

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
}

public enum DecisionType
{
    Action,
    Movement
}