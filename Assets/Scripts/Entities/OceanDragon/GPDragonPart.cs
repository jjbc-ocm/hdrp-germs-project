using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TanksMP;
using Photon.Pun;

public class GPDragonPart : GPMonsterBase
{
    DRAGON_STATES m_currentState = DRAGON_STATES.kIdle;
    float m_timeInState = 0.0f;
    public enum DRAGON_STATES
    {
        kIdle,
        kAttacking,
    }

    // Start is called before the first frame update
    void Start()
    {
        BaseStart();
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
        }

        if (m_health.m_isDead)
        {
            return;
        }

        LiveUpdate();

        m_timeInState += Time.deltaTime;
        switch (m_currentState)
        {
            case DRAGON_STATES.kIdle:
                OnIdleUpdate();
                break;
            case DRAGON_STATES.kAttacking:
                OnAttackingUpdate();
                break;
            default:
                break;
        }
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
                ChangeState(DRAGON_STATES.kAttacking);
            }

            if (!m_tweening)
            {
                LookAtTarget(m_currTargetPlayer.transform);
            }

        }
    }

    /// <summary>
    /// Logic to do while attacking.
    /// Returns to idle after the attack duration finishes.
    /// </summary>
    void OnAttackingUpdate()
    {
        MonsterAttackDesc attackDesc = null;

        if (m_currTargetPlayer == null)
        {
            ChangeState(DRAGON_STATES.kIdle);
            return;
        }

        Vector3 targetDir = m_currTargetPlayer.transform.position - transform.position;
        targetDir.y = 0.0f;
        if (targetDir.magnitude <= m_meleeRadius && m_meleeAtks.Count > 0) // pick melee attack
        {
            attackDesc = PickMeleeAttack();
        }
        else if (m_projectileAtks.Count > 0)//pick range attack
        {
            attackDesc = PickProjectileAttack();
        }

        if (attackDesc != null)
        {
            m_photonView.RPC("RPCPlayAnimationTrigger", RpcTarget.All, attackDesc.m_triggerName);
        }

        if (m_timeInState > m_currAtk.m_duration)
        {
            ChangeState(DRAGON_STATES.kIdle);
        }
    }

    /// <summary>
    /// Changes the state of the Dragon.
    /// </summary>
    /// <param name="newState"></param>
    void ChangeState(DRAGON_STATES newState)
    {
        if (newState == DRAGON_STATES.kIdle)
        {
            //choose new target
            ChoosePlayerToAttack();
            m_attacking = false;
        }
        else if (newState == DRAGON_STATES.kAttacking)
        {
            m_attacking = true;
        }

        m_timeInState = 0.0f;
        m_currentState = newState;
    }

}
