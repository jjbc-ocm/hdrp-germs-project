using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class GPGMessageWindow : GPGWindowUI
{
  public TextMeshProUGUI m_messageText;

  protected void Awake()
  {
    base.Awake();
  }

  public void SetMessage(string message)
  {
    m_messageText.text = message;
  }

}
