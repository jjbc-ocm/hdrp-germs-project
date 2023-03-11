using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class GPHealth : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("Health settings")]
    public float m_maxHealth = 10.0f;
    //[HideInInspector]
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

    /// <summary>
    /// Decreases the current health.
    /// Calls a OnDamagedEvent to which you can subcribe.
    /// </summary>
    /// <param name="damageAmount"></param>
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

    /// <summary>
    /// Inreases the current health.
    /// Calls a OnHealthChanged to which you can subcribe.
    /// </summary>
    /// <param name="healthAmount"></param>
    public void Heal(float healthAmount)
    {
        m_currentHealth += healthAmount;
        OnHealthChanged();
        if (OnHealEvent != null) { OnHealEvent.Invoke(); }
    }

    /// <summary>
    /// Set current health to be zero.
    /// Activates the m_isDead bool.
    /// Calls a OnDieEvent to which you can subcribe.
    /// </summary>
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

    /// <summary>
    /// Resets the current health to have the max health.
    /// Sets the m_isDead to be false.
    /// Calls a OnResurrectEvent to which you can subcribe.
    /// </summary>
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

    /// <summary>
    /// Resets the current health to have the max health, but does not resurrect.
    /// </summary>
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

    /// <summary>
    /// Sets the current health to a specific value.
    /// If value is equal or less than 0 then it is killed.
    /// </summary>
    /// <param name="value"></param>
    public void SetHealth(float value)
    {
        m_currentHealth = value;
        if (m_currentHealth <= 0)
        {
            Kill();
        }
        OnHealthChanged();
    }

    /// <summary>
    /// Get current health value.
    /// </summary>
    /// <returns></returns>
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
