using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPPunchTween : MonoBehaviour
{
    private Vector3 m_scale;
    public float m_scaleMultiplier = 1.8f;
    public float m_punchEffectDuration = 0.4f;
    public GameObject m_obj;

    private void Awake()
    {
        m_scale = m_obj.transform.localScale;
    }

    public void PunchEffect()
    {
        m_obj.transform.localScale = m_scale;
        LeanTween.scale(m_obj, m_scale * m_scaleMultiplier, m_punchEffectDuration).setEasePunch();
    }

    public void CustomPunchEffect(float multiplier, float duration)
    {
        m_obj.transform.localScale = m_scale;
        LeanTween.scale(m_obj, m_scale * multiplier, duration).setEasePunch();
    }
}
