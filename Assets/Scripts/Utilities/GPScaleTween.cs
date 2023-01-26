using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPScaleTween : MonoBehaviour
{
    private Vector3 m_originalScale;
    public float m_scaleMultiplier = 1.8f;
    [Tooltip("Gameobject to which apply the effect")]
    public GameObject m_obj;
    Vector3 m_targetScale;
    [Tooltip("Speed at which the GameObject will reach the target scale.")]
    public float m_scaleSpeed = 7.0f;

    // Start is called before the first frame update
    void Start()
    {
        //Store original local scale
        m_originalScale = m_obj.transform.localScale;
        //Initialize target scale using original scale for now so it
        //doesn't apply the effect until method is called.
        m_targetScale = m_originalScale;
    }

    private void Update()
    {
        //Scales the gameovject to the target scale.
        m_obj.transform.localScale = Vector3.Lerp(m_obj.transform.localScale, m_targetScale, Time.deltaTime * m_scaleSpeed);
    }

    /// <summary>
    /// Plays the scale effect.
    /// </summary>
    public void StartScaleFX()
    {
        m_targetScale = m_originalScale * m_scaleMultiplier;
    }

    /// <summary>
    /// Stops the scale effect.
    /// </summary>
    public void StopScaleFX()
    {
        m_targetScale = m_originalScale;
    }

}
