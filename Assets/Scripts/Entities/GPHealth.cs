using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class GPHealth : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("Health settings")]
    public float m_maxHealth = 10.0f;
    [HideInInspector]
    public float m_currentHealth = 10.0f;
    [HideInInspector]
    public bool m_isDead =false;

    [Header("Events")]
    public UnityEvent OnHealthChangedEvent;
    public UnityEvent OnDieEvent;
    public UnityEvent OnResurrectEvent;
    public UnityEvent OnHealEvent;
    public UnityEvent OnDamagedEvent;

    // Start is called before the first frame update
    void Start()
    {
        m_currentHealth = m_maxHealth;
    }

    public void Damage(float damageAmount)
    {
        m_currentHealth -= damageAmount;
        OnHealthChanged();

        if (m_currentHealth <= 0)
        {
            Kill();
        }

        if (OnDamagedEvent != null)
        {
            OnDamagedEvent.Invoke();
        }

    }

    public void Heal(float healthAmount)
    {
        m_currentHealth += healthAmount;
        OnHealthChanged();
        if (OnHealEvent != null) { OnHealEvent.Invoke(); }
    }

    public void Kill()
    {
        m_currentHealth = 0;
        OnHealthChanged();
        if (!m_isDead)
        {
            m_isDead = true;
            if (OnDieEvent != null) { OnDieEvent.Invoke(); }
        }
    }

    public void Resurrect()
    {
        m_currentHealth = m_maxHealth;
        OnHealthChanged();
        if (m_isDead)
        {
            m_isDead = false;
            if (OnResurrectEvent != null) { OnResurrectEvent.Invoke(); }
        }
    }

    public void ResetHealth()
    {
        m_currentHealth = m_maxHealth;
        OnHealthChanged();
    }

    void OnHealthChanged()
    {
        m_currentHealth = Mathf.Clamp(m_currentHealth, 0, m_maxHealth);
        if (OnHealthChangedEvent != null) { OnHealthChangedEvent.Invoke(); }
    }

    public void SetHealth(float value)
    {
        m_currentHealth = value;
        if (m_currentHealth <= 0)
        {
            Kill();
        }
        OnHealthChanged();
    }

    public float GetHealth()
    {
        return m_currentHealth;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(m_currentHealth);
        }
        else
        {
            SetHealth((float)stream.ReceiveNext());
        }
    }
}
