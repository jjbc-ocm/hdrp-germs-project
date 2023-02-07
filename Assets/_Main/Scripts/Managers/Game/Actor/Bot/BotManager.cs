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
            yield return new WaitForSeconds(1);

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
            if (entity is Player)
            {
                var targetPlayer = entity as Player;

                if (targetPlayer.GetTeam() == player.GetTeam())
                {
                    var newDecision = GetBestDecisionToAllyShip(targetPlayer);

                    currentDecision = GetBetterDecision(currentDecision, newDecision);
                }
                else
                {
                    var newDecision = GetBestDecisionToEnemyShip(targetPlayer);

                    currentDecision = GetBetterDecision(currentDecision, newDecision);
                }
            }

            if (entity is GPMonsterBase)
            {
                var targetMonster = entity as GPMonsterBase;
            }

            if (entity is Collectible)
            {
                // TODO:
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

    private DecisionNodeInfo GetBestDecisionToAllyShip(Player targetPlayer)
    {
        DecisionNodeInfo bestDecision = null;

        var weight = 0f;

        // Approach ally
        // Avoid ally

        bestDecision = new DecisionNodeInfo
        {
            Weight = weight,

            Decision = () =>
            {
                agent.SetDestination(targetPlayer.transform.position);
            }
        };

        return bestDecision;
    }

    private DecisionNodeInfo GetBestDecisionToEnemyShip(Player targetPlayer)
    {
        DecisionNodeInfo bestDecision = null;

        if (type == DecisionType.Action)
        {
            bestDecision = new DecisionNodeInfo
            {
                Weight = 1f,

                Decision = () =>
                {
                    player.Input.OnAttack(true);
                    // player.Bot.InputAttack();
                    //agent.SetDestination(targetPlayer.transform.position);
                }
            };
        }
        
        if (type == DecisionType.Movement)
        {
            bestDecision = new DecisionNodeInfo
            {
                Weight = GetWeightToEnemyShip(targetPlayer),

                Decision = () =>
                {
                    agent.SetDestination(targetPlayer.transform.position);

                    if (agent.path.corners.Length > 1)
                    {
                        var target = agent.path.corners[1];

                        var treshold = 0.05f;

                        var dotVertical = Vector3.Dot(player.transform.forward, (target - player.transform.position).normalized);

                        var dotHorizontal = Vector3.Dot(player.transform.right, (target - player.transform.position).normalized);

                        var x = dotHorizontal > treshold ? 1 : dotHorizontal < treshold ? -1 : 0;

                        var y = dotVertical > treshold ? 1 : dotVertical < treshold ? -1 : 0;

                        player.Input.OnMove(new Vector2(dotHorizontal, dotVertical));
                    }
                }
            };
        }

        

        return bestDecision;
    }

    private void GetBoestDecisionToMonster()
    {

    }

    private float GetWeightToEnemyShip(Player target)
    {
        // Make target more likely to be targeted if has low health

        // TODO: to add tomorrow
        // Increase weight if target is carrying a chest
        //
        return 1f - (target.Stat.Health / (float)target.Stat.MaxHealth);
    }
}

public enum DecisionType
{
    Action,
    Movement
}