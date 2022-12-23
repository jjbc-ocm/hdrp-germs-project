using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GPFriendWindow : MonoBehaviour
{
    public GPGUIScreen m_friendScreen;
    public GPGUIScreen m_addFriendScreen;

    public Button m_friendsTabButton;
    public Button m_addFriendTabButton;

    [Header("Tab Settings")]
    public Transform m_tabFocusImage;
    public Color m_selectedTabColor;
    public Color m_originalTabColor;

    GPGUIScreen m_currentScreen = null;

    [Header("Audio Settings")]
    public AudioClip m_changeTabSFX;

    // Start is called before the first frame update
    void Start()
    {
        m_friendsTabButton.onClick.AddListener(delegate { ShowFriendScreen(true); });
        m_addFriendTabButton.onClick.AddListener(delegate { ShowAddFriendScreen(true); });
        m_currentScreen = m_friendScreen;
        ShowFriendScreen(false);
    }

    public void ShowFriendScreen(bool playSFX)
    {
        m_currentScreen.Hide();
        m_friendScreen.Show();

        m_addFriendTabButton.GetComponent<TextMeshProUGUI>().color = m_originalTabColor; // set the old current tab to the original color

        m_currentScreen = m_friendScreen; // set new current tab

        MoveTapFocus(m_friendsTabButton.transform);
        m_friendsTabButton.GetComponent<TextMeshProUGUI>().color = m_selectedTabColor; // set selected color to new tab
        OnNewTabShown(playSFX);
    }

    public void ShowAddFriendScreen(bool playSFX)
    {
        m_currentScreen.Hide();
        m_addFriendScreen.Show();

        m_friendsTabButton.GetComponent<TextMeshProUGUI>().color = m_originalTabColor;

        m_currentScreen = m_addFriendScreen;

        MoveTapFocus(m_addFriendTabButton.transform);
        m_addFriendTabButton.GetComponent<TextMeshProUGUI>().color = m_selectedTabColor;
        OnNewTabShown(playSFX);
    }

    /// <summary>
    /// Logic to do when a new tab is shown.
    /// </summary>
    void OnNewTabShown(bool playSFX)
    {
        if (playSFX)
        {
            TanksMP.AudioManager.Play2D(m_changeTabSFX);
        }
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
