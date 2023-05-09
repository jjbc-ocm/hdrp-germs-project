using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class GPUITab
{
    public Button m_tabButton;
    public GPCustomizationTabScreen m_screen;
    public int idx = 0;
}

[System.Serializable]
public class GPDummyData
{
    public DummyPartSO m_skin;
    public DummyPartSO m_eye;
    public DummyPartSO m_mouth;
    public DummyPartSO m_hair;
    public DummyPartSO m_horns;
    public DummyPartSO m_wear;
    public DummyPartSO m_gloves;
    public DummyPartSO m_tail;
    public string m_dummyName;

    public DummyData ToDummyData()
    {
        return new DummyData
        {
            SkinID = m_skin?.ID ?? "",
            EyeID = m_eye?.ID ?? "",
            MouthID = m_mouth?.ID ?? "",
            HairID = m_hair?.ID ?? "",
            HornID = m_horns?.ID ?? "",
            WearID = m_wear?.ID ?? "",
            GloveID = m_gloves?.ID ?? "",
            TailID = m_tail?.ID ?? "",
            DummyName = m_dummyName ?? "",
        };
    }
}

public class GPDummyCustomizationScreen : GPGUIScreen
{
    [Header("Menu References")]
    public GPDummyScreen m_dummyScreen;

    [Header("Model Settings")]
    public GPDummySlotCard m_customizationSlot;

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
        //m_dummyScreen.m_selectedSlot.ReplaceModelObject(m_dummyScreen.m_dummyCustomizingScreen.m_customizationSlot.m_dummyModelRef);
        m_dummyScreen.SaveDummyChanges(m_dummyScreen.GetIdxOFSlot(m_dummyScreen.m_selectedSlot));
    }

    /// <summary>
    /// Switch the current customization tab to another one.
    /// </summary>
    /// <param name="idx"></param>
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

    /// <summary>
    /// Logic to do when a new tab is shown.
    /// </summary>
    void OnNewTabShown()
    {
        AudioManager.Instance.Play2D(m_changeTabSFX);
    }

    /// <summary>
    /// Animates selection tab sprite.
    /// </summary>
    /// <param name="targetTransform"></param>
    public void MoveTapFocus(Transform targetTransform)
    {
        LeanTween.move(m_tabFocusImage.gameObject, targetTransform.position, 0.3f).setEaseSpring();
    }

}
