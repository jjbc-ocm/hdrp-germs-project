using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Photon.Pun;

public class GPGWindowUI : MonoBehaviourPunCallbacks
{
  [Header("References")]
  public GameObject m_graphicsHolder;
  public Transform m_backPanel;
  public Button m_closeButton;

  [Header("Sound settings")]
  public AudioClip m_showSound;
  public AudioClip m_hideSound;

  [Header("Tween settings")]
  public bool m_useTween = true;
  public LeanTweenType m_showingTween = LeanTweenType.easeInOutCirc;
  public LeanTweenType m_hidingTween = LeanTweenType.easeInOutCirc;
  public float m_tweenDuration = 0.3f;
  public float m_tweenHideDelay = 0.0f;
  public bool m_tweenX = true;
  public bool m_tweenY = true;

  [Header("Events")]
  public bool m_useWindowManager = false;
  public UnityEvent OnShowEvent;
  public UnityEvent OnHideEvent;
  public UnityEvent OnCloseEvent;

  public static event Action <GPGWindowUI> OnShowGlobalEvent;
  public static event Action <GPGWindowUI> OnHideGlobalEvent;

  [HideInInspector]
  public bool m_opened = false;
  [HideInInspector]
  public bool m_pendingClose = false;

  Vector3 m_initialLocalScale;

  protected void Awake()
  {
    
    if (!m_backPanel)
    {
      m_backPanel = transform.Find("Panel");
    }

    if (!m_opened)
    {
      if (m_backPanel)
      {
        m_backPanel.gameObject.SetActive(false);
      }
    }

    m_initialLocalScale = m_graphicsHolder.transform.localScale;
  }

  public virtual void Show()
  {
    if (m_opened || m_pendingClose)
    {
      return;
    }

    m_opened = true;
    m_pendingClose = false;

    m_graphicsHolder.SetActive(true);
    if (m_backPanel)
    {
      m_backPanel.gameObject.SetActive(true);
    }

    /*
    if (CDAudioManager.instance)
    {
      CDAudioManager.instance.PlaySound(CD_AUDIOSOURCES.kUI, m_showSound);
    }
    */

    if (m_useTween)
    {
      Vector3 startScale = m_graphicsHolder.transform.localScale;
      if (m_tweenX)
      {
        LeanTween.scaleX(m_graphicsHolder, m_initialLocalScale.x, m_tweenDuration).setEase(m_showingTween);
        startScale.x = 0;
      }
      if (m_tweenY)
      {
        LeanTween.scaleY(m_graphicsHolder, m_initialLocalScale.y, m_tweenDuration).setEase(m_showingTween);
        startScale.y = 0;
      }
      m_graphicsHolder.transform.localScale = startScale;
    }

    if (OnShowEvent != null)
    {
      OnShowEvent.Invoke();
    }

    if (OnShowGlobalEvent != null)
    {
      OnShowGlobalEvent.Invoke(this);
    }

  }

  public virtual void Hide()
  {
    /*
    if (CDAudioManager.instance)
    {
      CDAudioManager.instance.PlaySound(CD_AUDIOSOURCES.kUI, m_hideSound);
    }
    */

    if (OnHideEvent != null)
    {
      OnHideEvent.Invoke();
    }

    if (m_useTween && m_graphicsHolder)
    {
      if (m_tweenX)
      {
        LeanTween.scaleX(m_graphicsHolder, 0, m_tweenDuration).setEase(m_hidingTween).setDelay(m_tweenHideDelay).setOnComplete(Disable);
      }
      if (m_tweenY)
      {
        LeanTween.scaleY(m_graphicsHolder, 0, m_tweenDuration).setEase(m_hidingTween).setDelay(m_tweenHideDelay).setOnComplete(Disable);
      }
      m_pendingClose = true;
    }
    else
    {
      m_pendingClose = true;
      Disable();
    }

  }

  public void Disable()
  {
    if (m_pendingClose && m_graphicsHolder)
    {
      m_graphicsHolder.SetActive(false);
      if (m_backPanel)
      {
        m_backPanel.gameObject.SetActive(false);
      }

      m_pendingClose = false;
      m_opened = false;

      if (OnCloseEvent != null)
      {
        OnCloseEvent.Invoke();
      }

      if (OnHideGlobalEvent != null)
      {
        OnHideGlobalEvent.Invoke(this);
      }
    }
  }

  public void TransitionToOtherWindow(GPGWindowUI window)
  {
    StartCoroutine(IETransitionToOtherWindow(window));
  }

  IEnumerator IETransitionToOtherWindow(GPGWindowUI window)
  {
    Hide();
    yield return new WaitForSeconds(m_tweenHideDelay + m_tweenDuration - Time.deltaTime);
    window.Show();
  }

}
