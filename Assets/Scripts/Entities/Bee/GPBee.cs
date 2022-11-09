using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TanksMP;
using Photon.Pun;

public class GPBee : GPMonsterBase
{
    BEE_STATES m_currentState = BEE_STATES.kIdle;
    float m_timeInState = 0.0f;

    public enum BEE_STATES
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
        if (m_health.m_isDead)
        {
            DeathUpdate();
        }

        //Only master client will execute enemy logic and share the data to other players
        if (!PhotonNetwork.IsMasterClient || m_health.m_isDead)
        {
            return;
        }

        LiveUpdate();

        m_timeInState += Time.deltaTime;
        switch (m_currentState)
        {
            case BEE_STATES.kIdle:
                OnIdle();
                break;
            case BEE_STATES.kAttacking:
                OnAttacking();
                break;
            default:
                break;
        }
    }

    void OnIdle()
    {
        if (m_currTargetPlayer != null)
        {
            m_nextAttackTimeCounter += Time.deltaTime;
            if (m_nextAttackTimeCounter > m_nextAttackTime)
            {
                m_nextAttackTimeCounter = 0.0f;
                m_nextAttackTime = Random.Range(m_minAttackTime, m_maxAttackTime);
                ChangeState(BEE_STATES.kAttacking);
            }

            if (!m_tweening)
            {
                LookAtTarget(m_currTargetPlayer.transform);
            }

        }
    }

    void OnAttacking()
    {
        MonsterAttackDesc attackDesc = null;

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

        m_photonView.RPC("RPCPlayAnimationTrigger", RpcTarget.All, attackDesc.m_triggerName);

        if (m_timeInState > m_currAtk.m_duration)
        {
            ChangeState(BEE_STATES.kIdle);
        }
    }

    void ChangeState(BEE_STATES newState)
    {
        if (m_currentState == BEE_STATES.kIdle && newState == BEE_STATES.kAttacking)
        {
            //look at target
            /*
            Vector3 lookDir = m_currTargetPlayer.transform.position - transform.position;
            lookDir.y = 0.0f;
            m_tweening = true;
            LeanTween.rotate(gameObject, Quaternion.LookRotation(lookDir).eulerAngles, 0.5f).setEaseSpring().setOnComplete(OnTweenEnd);
            */
        }
        else if (m_currentState == BEE_STATES.kAttacking && newState == BEE_STATES.kIdle)
        {
            //choose new target
            ChoosePlayerToAttack();
        }

        m_timeInState = 0.0f;
        m_currentState = newState;
    }

}
