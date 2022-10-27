using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TanksMP;
using Photon.Pun;

public class GPMonsterBase : MonoBehaviour
{
    [Header("Component references")]
    public GPHealth m_health;
    public GPGTriggerEvent m_detectionTrigger;

    [Header("Movement settings")]
    public float m_rotateSpeed = 3.0f;
    [HideInInspector]
    public bool m_tweening = false;

    [Header("Animation settings")]
    public Animator m_animator;
    public string m_attackTriggerName = "Bite Attack";
    public string m_dieTriggerName = "Die";
    public string m_hurtTriggerName = "Take damage";

    [Header("Attack settings")]
    public float m_attackRadius = 3.0f;
    public float m_minAttackTime = 1.0f;
    public float m_maxAttackTime = 3.5f;
    [HideInInspector]
    public float m_nextAttackTime = 0.0f;
    [HideInInspector]
    public float m_nextAttackTimeCounter = 0.0f;
    public Player m_currTargetPlayer;
   
    [HideInInspector]
    public List<Player> m_playersInRange = new List<Player>();
    [HideInInspector]
    public List<Player> m_playersWhoDamageIt = new List<Player>();
    

    public virtual void ChoosePlayerToAttack()
    {
        int playerIdx = Random.Range(0, m_playersInRange.Count);
        m_currTargetPlayer = m_playersInRange[playerIdx];
    }

    public virtual void OnDamage()
    {
        m_animator.SetTrigger(m_hurtTriggerName);
    }

    public virtual void OnDie()
    {
        m_animator.SetTrigger(m_dieTriggerName);
    }

    public virtual void DamageMonster(Bullet bullet)
    {
        m_health.Damage(bullet.damage);

        Player other = bullet.owner.GetComponent<Player>();
        if (other)
        {
            if (!m_playersWhoDamageIt.Contains(other))
            {
                m_playersWhoDamageIt.Add(other);
            }
        }
        
    }

    public virtual void OnPlayerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player)
        {
            if (!m_playersInRange.Contains(player))
            {
                m_playersInRange.Add(player);
            }
            m_currTargetPlayer = player;
        }
    }

    public virtual void OnPlayerExit(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player)
        {
            if (m_playersInRange.Contains(player))
            {
                m_playersInRange.Remove(player);
            }
        }

        if (player == m_currTargetPlayer)
        {
            m_currTargetPlayer = null;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, m_attackRadius);
    }
}
