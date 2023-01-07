using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GPStoreScreen : GPGUIScreen
{
    [Header("Tab buttons")]
    public Button m_chestButton;
    public Button m_ticketsButton;
    public Button m_rollButton;
    public Button m_homeButton;
    Button m_currentTabButton = null;

    [Header("Screen references")]
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
        //Suscribe to tab button events
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

    public void LockButtons(bool locked)
    {
        m_chestButton.interactable = !locked;
        m_ticketsButton.interactable = !locked;
        m_rollButton.interactable = !locked;
        m_homeButton.interactable = !locked;
    }

    /// <summary>
    /// Displays the chest screen.
    /// </summary>
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

    /// <summary>
    /// Displays the tickets screen.
    /// </summary>
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

    /// <summary>
    /// Displays the Spin wheel screen.
    /// </summary>
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

    /// <summary>
    /// Logic to do when a new tab is shown.
    /// </summary>
    void OnNewTabShown()
    {
        TanksMP.AudioManager.Play2D(m_changeTabSFX);
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
