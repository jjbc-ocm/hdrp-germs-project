using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GPHealthBar : MonoBehaviour
{
    public Image m_bar;
    public GPHealth m_health;
    public UnityEvent m_onDamaged;

    // Start is called before the first frame update
    void Start()
    {
        m_health.OnHealthChangedEvent.AddListener(UpdateBar);
        m_health.OnDamagedEvent.AddListener(OnDamaged);
    }

    void UpdateBar()
    {
        m_bar.fillAmount = m_health.m_currentHealth / m_health.m_maxHealth;
    }

    void OnDamaged()
    {
        if (m_onDamaged != null)
        {
            m_onDamaged.Invoke();
        }
    }
}
