using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GPStoreScreen : GPGUIScreen
{
    public Button m_chestButton;
    public Button m_ticketsButton;
    public Button m_rollButton;
    Button m_currentTabButton = null;

    public GPChestScreen m_chestScreen;
    public GPTicketScreen m_ticketsScreen;
    public GPRollScreen m_rollScreen;
    GPGUIScreen m_currentScreen = null;

    [Header("Tab Settings")]
    public Transform m_tabFocusImage;
    public Color m_selectedTabColor;
    public Color m_originalTabColor;

    [Header("Audio Settings")]
    public AudioClip m_changeTabSFX;

    // Start is called before the first frame update
    void Start()
    {
        m_chestButton.onClick.AddListener(ShowChestScreen);
        m_ticketsButton.onClick.AddListener(ShowTicketScreen);
        m_rollButton.onClick.AddListener(ShowRollScreen);
    }

    public override void Show()
    {
        base.Show();
        m_currentScreen = m_chestScreen;
        m_currentTabButton = m_chestButton;
        ShowChestScreen();
    }

    public override void Hide()
    {
        base.Hide();
        ShowChestScreen(); // so it's the one selected when the user opens the store again.
    }

    public void ShowChestScreen()
    {
        m_currentScreen.Hide();
        m_chestScreen.Show();

        m_currentScreen = m_chestScreen;

        MoveTapFocus(m_chestButton.transform);
        m_currentTabButton.GetComponent<TextMeshProUGUI>().color = m_originalTabColor;
        m_chestButton.GetComponent<TextMeshProUGUI>().color = m_selectedTabColor;
        m_currentTabButton = m_chestButton;
        OnNewTabShown();
    }

    public void ShowTicketScreen()
    {
        m_currentScreen.Hide();
        m_ticketsScreen.Show();

        m_currentScreen = m_ticketsScreen;

        MoveTapFocus(m_ticketsButton.transform);

        m_currentTabButton.GetComponent<TextMeshProUGUI>().color = m_originalTabColor;
        m_ticketsButton.GetComponent<TextMeshProUGUI>().color = m_selectedTabColor;

        m_currentTabButton = m_ticketsButton;
        OnNewTabShown();
    }

    public void ShowRollScreen()
    {
        m_currentScreen.Hide();
        m_rollScreen.Show();

        m_currentScreen = m_rollScreen;

        MoveTapFocus(m_rollButton.transform);

        m_currentTabButton.GetComponent<TextMeshProUGUI>().color = m_originalTabColor;
        m_rollButton.GetComponent<TextMeshProUGUI>().color = m_selectedTabColor;
        m_currentTabButton = m_rollButton;
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
