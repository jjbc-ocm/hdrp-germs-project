using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GPGTriggerEvent : MonoBehaviour
{
    public UnityEvent<Collider> m_OnEnterEvent;
    public UnityEvent<Collider> m_OnExitEvent;
    public List<string> m_TagsToCheck;
    Collider m_collider;

    private void Start()
    {
        m_collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_TagsToCheck.Contains(other.tag) || m_TagsToCheck.Count == 0)
        {
            m_OnEnterEvent.Invoke(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_TagsToCheck.Contains(other.tag) || m_TagsToCheck.Count == 0)
        {
            m_OnExitEvent.Invoke(other);
        }
    }

    public void SetEnabled(bool enabled)
    {
        if (m_collider)
        {
            m_collider.enabled = enabled;
        }
    }
}
