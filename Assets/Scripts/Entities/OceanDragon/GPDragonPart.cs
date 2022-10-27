using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TanksMP;
using Photon.Pun;

public class GPDragonPart : GPMonsterBase
{
    DRAGON_STATES m_currentState = DRAGON_STATES.kIdle;
    public enum DRAGON_STATES
    {
        kIdle,
        kAttacking,
    }

    // Start is called before the first frame update
    void Start()
    {
        m_health.OnDieEvent.AddListener(OnDie);
        m_health.OnDamagedEvent.AddListener(OnDamage);
        m_detectionTrigger.m_OnEnterEvent.AddListener(OnPlayerEnter);
        m_detectionTrigger.m_OnExitEvent.AddListener(OnPlayerExit);
    }

    // Update is called once per frame
    void Update()
    {
        //Only master client will execute enemy logic and share the data to other players
        if (!PhotonNetwork.IsMasterClient || m_health.m_isDead)
        {
            return;
        }

        switch (m_currentState)
        {
            case DRAGON_STATES.kIdle:
                OnIdle();
                break;
            case DRAGON_STATES.kAttacking:
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
                ChangeState(DRAGON_STATES.kAttacking);
            }

            if (!m_tweening)
            {
                Vector3 lookDir = m_currTargetPlayer.transform.position - transform.position;
                lookDir.y = 0.0f;
                transform.forward = Vector3.Lerp(transform.forward, lookDir, Time.deltaTime * m_rotateSpeed);
            }

        }
    }

    void OnAttacking()
    {
        m_animator.SetTrigger(m_attackTriggerName);
        ChangeState(DRAGON_STATES.kIdle);
    }

    void ChangeState(DRAGON_STATES newState)
    {
        if (m_currentState == DRAGON_STATES.kIdle && newState == DRAGON_STATES.kAttacking)
        {


            //look at target
            /*
            Vector3 lookDir = m_currTargetPlayer.transform.position - transform.position;
            lookDir.y = 0.0f;
            m_tweening = true;
            LeanTween.rotate(gameObject, Quaternion.LookRotation(lookDir).eulerAngles, 0.5f).setEaseSpring().setOnComplete(OnTweenEnd);
            */
        }
        else if (m_currentState == DRAGON_STATES.kAttacking && newState == DRAGON_STATES.kIdle)
        {
            //choose new target
            ChoosePlayerToAttack();
        }

        m_currentState = newState;
    }

    void OnTweenEnd()
    {
        m_tweening = false;
    }
    
}
