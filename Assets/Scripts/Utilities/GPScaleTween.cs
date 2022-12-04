using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPScaleTween : MonoBehaviour
{
    private Vector3 m_originalScale;
    public float m_scaleMultiplier = 1.8f;
    public GameObject m_obj;
    Vector3 m_targetScale;
    public float m_scaleSpeed = 7.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_originalScale = m_obj.transform.localScale;
        m_targetScale = m_originalScale;
    }

    private void Update()
    {
        m_obj.transform.localScale = Vector3.Lerp(m_obj.transform.localScale, m_targetScale, Time.deltaTime * m_scaleSpeed);
    }

    public void StartScaleFX()
    {
        m_targetScale = m_originalScale * m_scaleMultiplier;
    }

    public void StopScaleFX()
    {
        m_targetScale = m_originalScale;
    }

}
