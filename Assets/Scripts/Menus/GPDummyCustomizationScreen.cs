using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class GPUITab
{
  public Button m_tabButton;
  public GPGUIScreen m_screen;
  public int idx = 0;
}

public class GPDummyCustomizationScreen : GPGUIScreen
{
  [Header("Tab Settings")]
  public Transform m_tabFocusImage;
  public Color m_selectedTabColor;
  public Color m_originalTabColor;
  public List<GPUITab> m_tabs;

  GPUITab m_currentTab = null;

  [Header("Audio Settings")]
  public AudioClip m_changeTabSFX;

  private void Start()
  {
    for (int i = 0; i < m_tabs.Count; i++)
    {
      int x = i;
      m_tabs[i].m_tabButton.onClick.AddListener(delegate { ShowTab(x); });
    }
  }

  public override void Show()
  {
    base.Show();
    m_currentTab = m_tabs[0];
    ShowTab(0);
  }

  public override void Hide()
  {
    base.Hide();
    ShowTab(0); // so it's the one selected when the user opens the customization again.
  }

  public void ShowTab(int idx)
  {
    m_currentTab.m_screen.Hide();
    m_tabs[idx].m_screen.Show();

    m_currentTab.m_tabButton.GetComponent<TextMeshProUGUI>().color = m_originalTabColor;

    m_currentTab = m_tabs[idx];

    MoveTapFocus(m_currentTab.m_tabButton.transform);
    m_tabs[idx].m_tabButton.GetComponent<TextMeshProUGUI>().color = m_selectedTabColor;
    OnNewTabShown();
  }
  void OnNewTabShown()
  {
    TanksMP.AudioManager.Play2D(m_changeTabSFX);
  }

  public void MoveTapFocus(Transform targetTransform)
  {
    LeanTween.move(m_tabFocusImage.gameObject, targetTransform.position, 0.3f).setEaseSpring();
  }
}
