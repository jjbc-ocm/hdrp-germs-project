using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPLoopScaler : MonoBehaviour
{
  public LeanTweenType m_tweenType;
  public float m_tweenDuration = 0.2f;
  public float m_targetScaleX = 1.4f;
  public float m_targetScaleY = 1.4f;

  // Start is called before the first frame update
  void Start()
  {
    LeanTween.scaleY(gameObject, m_targetScaleY, m_tweenDuration).setEase(m_tweenType).setLoopPingPong();
    LeanTween.scaleX(gameObject, m_targetScaleX, m_tweenDuration).setEase(m_tweenType).setLoopPingPong();
  }
}
