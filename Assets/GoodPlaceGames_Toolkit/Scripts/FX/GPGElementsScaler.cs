using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPGElementsScaler : MonoBehaviour
{
  public List<GameObject> m_elementsToScale;
  List<Vector3> m_elementsOriginalScale = new List<Vector3>();
  public LeanTweenType m_tweenType;
  public float m_tweenDuration = 0.2f;
  public float m_delayBetweenElements = 0.2f;
  public bool m_playOnStart = true;
  public float m_animDelay = 0.0f;
  public bool m_scaleX = true;
  public bool m_scaleY = true;
  public bool m_scaleZ = true;

  // Start is called before the first frame update
  void Start()
  {
    
  }

  private void OnEnable()
  {
    for (int i = 0; i < m_elementsToScale.Count; i++)
    {
      m_elementsOriginalScale.Add(m_elementsToScale[i].transform.localScale);
    }

    if (m_playOnStart)
    {
      StartAnimation(false);
    }
  }

  public void StartAnimation(bool backwards = false)
  {
    if (gameObject.activeSelf)
    {
      StartCoroutine(IEStartAnimation(backwards));
    }
  }

  public IEnumerator IEStartAnimation(bool backwards)
  {
    for (int i = 0; i < m_elementsToScale.Count; i++)
    {
      if (!m_elementsToScale[i].activeSelf) //skip disabled objects
      {
        continue;
      }

      if (backwards)
      {
        m_elementsToScale[i].transform.localScale = m_elementsOriginalScale[i];
      }
      else
      {
        Vector3 startScale = m_elementsOriginalScale[i];
        if (m_scaleX)
        {
          startScale.x = 0.0f;
        }
        if (m_scaleY)
        {
          startScale.y = 0.0f;
        }
        if (m_scaleZ)
        {
          startScale.z = 0.0f;
        }
        m_elementsToScale[i].transform.localScale = startScale;
      }
    }

    yield return new WaitForSeconds(m_animDelay);

    for (int i = 0; i < m_elementsToScale.Count; i++)
    {
      int idx = i;
      Vector3 targetScale = m_elementsOriginalScale[idx];
      if (backwards)
      {
        idx = m_elementsToScale.Count - i - 1;
        targetScale = Vector3.zero;
      }

      if (m_scaleX)
      {
        LeanTween.scaleX(m_elementsToScale[idx], targetScale.x, m_tweenDuration).setEase(m_tweenType).setDelay(m_delayBetweenElements);
      }
      if (m_scaleY)
      {
        LeanTween.scaleY(m_elementsToScale[idx], targetScale.y, m_tweenDuration).setEase(m_tweenType).setDelay(m_delayBetweenElements);
      }
      if (m_scaleZ)
      {
        LeanTween.scaleZ(m_elementsToScale[idx], targetScale.z, m_tweenDuration).setEase(m_tweenType).setDelay(m_delayBetweenElements);
      }

      if (m_delayBetweenElements > 0)
      {
        yield return new WaitForSeconds(m_delayBetweenElements);
      }
    }
  }

}
