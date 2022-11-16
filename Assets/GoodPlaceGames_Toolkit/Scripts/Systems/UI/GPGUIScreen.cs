using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class GPGUIScreen : MonoBehaviourPunCallbacks
{
  public GameObject m_screenHolder;
  public UnityEvent OnShowEvent;
  public UnityEvent OnHideEvent;
  public bool m_visible = false;

  private void Awake()
  {
    m_visible = m_screenHolder.activeSelf;
  }

  public virtual void Show()
  {
    if (m_visible) { return; }
    m_screenHolder.SetActive(true);
    m_visible = true;

    if (OnShowEvent != null)
    {
      OnShowEvent.Invoke();
    }
  }

  public virtual void Hide()
  {
    if (!m_visible) { return; }

    m_screenHolder.SetActive(false);
    m_visible = false;

    if (OnHideEvent != null)
    {
      OnHideEvent.Invoke();
    }
  }
}
