using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPPunchTween : MonoBehaviour
{
    private Vector3 m_scale;
    [Tooltip("How big(or small if less than 1.0f) the game object will get in relation to its original size")]
    public float m_scaleMultiplier = 1.8f;
    [Tooltip("Duration of the effect")]
    public float m_punchEffectDuration = 0.4f;
    [Tooltip("GameObject to which apply the effect")]
    public GameObject m_obj;

    private void Awake()
    {
        //Store original local scale
        m_scale = m_obj.transform.localScale;
    }

    /// <summary>
    /// Plays punch effect using the settings setted in the component
    /// </summary>
    public void PunchEffect()
    {
        //Reset original scale
        m_obj.transform.localScale = m_scale;
        //Play punch effect.
        LeanTween.scale(m_obj, m_scale * m_scaleMultiplier, m_punchEffectDuration).setEasePunch();
    }

    /// <summary>
    /// Plays punch effect using custom settiings.
    /// </summary>
    /// <param name="multiplier"></param>
    /// <param name="duration"></param>
    public void CustomPunchEffect(float multiplier, float duration)
    {
        m_obj.transform.localScale = m_scale;
        LeanTween.scale(m_obj, m_scale * multiplier, duration).setEasePunch();
    }
}
