using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GPGLoadingCircle : MonoBehaviour
{
  public RectTransform m_circle;
  public float m_timeStep;
  public float m_oneStepAngle;
  float m_startTime;

  // Start is called before the first frame update
  void Start()
  {
    m_startTime = Time.time;
  }

  // Update is called once per frame
  void Update()
  {
    if (Time.time - m_startTime >= m_timeStep)
    {
      Vector3 iconAngle = m_circle.localEulerAngles;
      iconAngle.z += m_oneStepAngle;
      m_circle.localEulerAngles = iconAngle;
      m_startTime = Time.time;
    }
  }
}
