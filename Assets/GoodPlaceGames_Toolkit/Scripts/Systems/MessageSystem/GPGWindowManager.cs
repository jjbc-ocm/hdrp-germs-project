using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPGWindowManager : MonoBehaviour
{
  public List<GPGWindowUI> m_windows;
  public GPGWindowUI m_currentWindow;

  // Start is called before the first frame update
  void Start()
  {
    m_windows.AddRange(GameObject.FindObjectsOfType<GPGWindowUI>(true));
    //for (int i = 0; i < m_windows.Count; i++)
    //{
    //}

    GPGWindowUI.OnShowGlobalEvent += OnShowWindow;
    GPGWindowUI.OnHideGlobalEvent += OnHideWindow;
  }

  void OnShowWindow(GPGWindowUI window)
  {
    if (!window.m_useWindowManager)
    {
      return;
    }

    if (!m_windows.Contains(window))
    {
      m_windows.Add(window);
    }
    m_currentWindow = window;
    CloseOtherWindows();
  }

  void OnHideWindow(GPGWindowUI window)
  {
    if (m_currentWindow == window && window.m_useWindowManager)
    {
      m_currentWindow = null;
    }
  }

  public void CloseOtherWindows()
  {
    for (int i = 0; i < m_windows.Count; i++)
    {
      if (m_currentWindow != m_windows[i] && m_windows[i].m_opened && !m_windows[i].m_pendingClose && m_windows[i].m_useWindowManager)
      {
        m_windows[i].Hide();
      }
    }
  }

}
