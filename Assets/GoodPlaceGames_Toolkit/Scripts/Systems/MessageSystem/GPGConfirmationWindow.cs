using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class GPGConfirmationWindow : GPGMessageWindow
{
  public TextMeshProUGUI m_messageHeadline;
  public UnityEvent OnConfirmEvent;
  public UnityEvent OnCancelEvent;

  protected void Awake()
  {
    base.Awake();
  }

  public void SetHeadline(string headline)
  {
    m_messageHeadline.text = headline;
  }

  public void OnConfirm()
  {
    if (OnConfirmEvent != null)
    {
      OnConfirmEvent.Invoke();
    }
    Hide();
  }

  public void OnCancel()
  {
    if (OnCancelEvent != null)
    {
      OnCancelEvent.Invoke();
    }
    Hide();
  }
}
