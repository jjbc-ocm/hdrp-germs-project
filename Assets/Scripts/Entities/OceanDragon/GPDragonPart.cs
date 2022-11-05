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
        if (PhotonNetwork.IsMasterClient)
        {
            m_health.OnDieEvent.AddListener(OnDie);
            m_health.OnDamagedEvent.AddListener(OnDamage);
            m_detectionTrigger.m_OnEnterEvent.AddListener(OnPlayerEnter);
            m_detectionTrigger.m_OnExitEvent.AddListener(OnPlayerExit);
            m_meleeDamageTrigger.m_OnEnterEvent.AddListener(BitePlayer);
        }
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
                LookAtTarget(m_currTargetPlayer.transform);
            }

        }
    }

    void OnAttacking()
    {
        string triggerToActivate = "";

        Vector3 targetDir = m_currTargetPlayer.transform.position - transform.position;
        targetDir.y = 0.0f;
        if (targetDir.magnitude <= m_meleeRadius) // pick melee attack
        {
            triggerToActivate = PickAttack(m_meleeAtkTriggerNames);
        }
        else if (m_projectileAtkTriggerNames.Count > 0)//pick range attack
        {
            triggerToActivate = PickAttack(m_projectileAtkTriggerNames);
        }

        m_animator.SetTrigger(triggerToActivate);
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

    public void BitePlayer(Collider collider)
    {
        Player player = collider.GetComponent<Player>();
        if (player && player == m_currTargetPlayer)
        {
            player.TakeMonsterDamage(this);
        }

        EndAttack();
    }

}
