using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TanksMP;
using Photon.Pun;

public class GPBee : GPMonsterBase
{
    BEE_STATES m_currentState = BEE_STATES.kIdle;
    float m_timeInState = 0.0f;
    public NavMeshAgent m_agent;

    MonsterAttackDesc m_nextAttackDesc = null;

    public enum BEE_STATES
    {
        kIdle,
        kAttacking,
        kChase,
        kReturning,
    }

    // Start is called before the first frame update
    void Start()
    {
        BaseStart();
        if (!PhotonNetwork.IsMasterClient)
        {
            m_agent.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Update visuals
        if (Player.Mine != null)
        {
            var isInPlayerRange = Vector3.Distance(transform.position, Player.Mine.transform.position) <= Constants.FOG_OF_WAR_DISTANCE;

            foreach (var m_renderer in m_renderers)
            {
                m_renderer.SetActive(isInPlayerRange || IsInSupremacyWard());
            }
        }

        //Only master client will execute enemy logic and share the data to other players
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (m_health.m_isDead)
        {
            DeathUpdate();
            m_agent.isStopped = true;
        }
        else
        {
            m_agent.isStopped = false;
        }

        if (m_health.m_isDead)
        {
            return;
        }

        LiveUpdate();

        //return to nest after too far away
        Vector3 nestsDir = m_respawnWorldPosition - transform.position;
        nestsDir.y = 0.0f;
        if (m_currentState != BEE_STATES.kReturning && m_currTargetPlayer == null && nestsDir.magnitude > 2.0f)
        {
            ReturnToNest();
        }

        m_timeInState += Time.deltaTime;
        switch (m_currentState)
        {
            case BEE_STATES.kIdle:
                OnIdleUpdate();
                break;
            case BEE_STATES.kAttacking:
                OnAttackingUpdate();
                break;
            case BEE_STATES.kChase:
                OnChaseUpdate();
                break;
            case BEE_STATES.kReturning:
                OnReturningUpdate();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Changes the state of the bee.
    /// </summary>
    /// <param name="newState"></param>
    void ChangeState(BEE_STATES newState)
    {
        if (newState == BEE_STATES.kAttacking)
        {
            OnStartAttacking();
        }
        else if (newState == BEE_STATES.kIdle)
        {
            OnStartIdle();
        }
        else if (newState == BEE_STATES.kChase)
        {
            OnStartChase();
        }
        else if (newState == BEE_STATES.kReturning)
        {
            OnStartReturning();
        }

        m_timeInState = 0.0f;
        m_currentState = newState;
    }

    /// <summary>
    /// Logic to do at the start of being idle.
    /// </summary>
    void OnStartIdle()
    {
        //choose new target
        ChoosePlayerToAttack();
        m_attacking = false;
    }

    /// <summary>
    /// Logic to do every frame while being idle.
    /// </summary>
    void OnIdleUpdate()
    {
        if (m_currTargetPlayer != null)
        {
            m_nextAttackTimeCounter += Time.deltaTime;
            if (m_nextAttackTimeCounter > m_nextAttackTime)
            {
                m_nextAttackTimeCounter = 0.0f;
                m_nextAttackTime = Random.Range(m_minAttackTime, m_maxAttackTime);
                ChangeState(BEE_STATES.kAttacking);

                Vector3 targetDir = m_currTargetPlayer.transform.position - transform.position;
                targetDir.y = 0.0f;
                if (targetDir.magnitude < m_meleeRadius)
                {
                    ChangeState(BEE_STATES.kAttacking);
                }
                else if (targetDir.magnitude < m_projectileRadius)
                {
                    ChangeState(BEE_STATES.kAttacking);
                }
                else
                {
                    ChangeState(BEE_STATES.kChase);
                }
            }

            if (!m_tweening)
            {
                LookAtTarget(m_currTargetPlayer.transform);
            }

        }
    }

    /// <summary>
    /// Logic to do before start attacking.
    /// </summary>
    void OnStartAttacking()
    {
        if (m_currTargetPlayer == null)
        {
            ChangeState(BEE_STATES.kIdle);
            return;
        }

        m_attacking = true;

        //choose next attack
        Vector3 targetDir = m_currTargetPlayer.transform.position - transform.position;
        targetDir.y = 0.0f;
        if (targetDir.magnitude < m_meleeRadius && m_meleeAtks.Count > 0) // pick melee attack
        {
            m_nextAttackDesc = PickMeleeAttack();
        }
        else if (targetDir.magnitude < m_projectileRadius && m_projectileAtks.Count > 0)//pick range attack
        {
            m_nextAttackDesc = PickProjectileAttack();
        }

        if (m_nextAttackDesc != null)
        {
            m_photonView.RPC("RPCPlayAnimationTrigger", RpcTarget.All, m_nextAttackDesc.m_triggerName);
        }
    }

    /// <summary>
    /// Logic to do while attacking.
    /// Returns to idle after the attack duration finishes.
    /// </summary>
    void OnAttackingUpdate()
    {
        if (m_timeInState > m_currAtk.m_duration)
        {
            ChangeState(BEE_STATES.kIdle);
        }
    }

    /// <summary>
    /// Logic to do when starting to chase a player
    /// </summary>
    void OnStartChase()
    {

    }

    /// <summary>
    /// Logic to do whle chasing a player.
    /// </summary>
    void OnChaseUpdate()
    {
        if (m_currTargetPlayer == null)
        {
            ChangeState(BEE_STATES.kIdle);
            return;
        }

        m_agent.SetDestination(m_currTargetPlayer.transform.position);

        Vector3 targetDir = m_currTargetPlayer.transform.position - transform.position;
        targetDir.y = 0.0f;
        if (targetDir.magnitude < m_meleeRadius)
        {
            ChangeState(BEE_STATES.kAttacking);
        }

        if (!m_tweening)
        {
            LookAtTarget(m_currTargetPlayer.transform);
        }
    }

    /// <summary>
    /// Logic to do when starting to return to nest.
    /// </summary>
    void OnStartReturning()
    {

    }


    /// <summary>
    /// Logic to do while returning to nest.
    /// Moves monster towards his respawn point (nest)
    /// </summary>
    void OnReturningUpdate()
    {
        m_agent.SetDestination(m_respawnWorldPosition);

        Vector3 targetDir = m_respawnWorldPosition - transform.position;
        targetDir.y = 0.0f;

        if (targetDir.magnitude < 2.0f)
        {
            ChangeState(BEE_STATES.kIdle);
        }
    }

    /// <summary>
    /// Call to make the mosnter return to his nest.
    /// </summary>
    public void ReturnToNest()
    {
        ChangeState(BEE_STATES.kReturning);
    }

}
