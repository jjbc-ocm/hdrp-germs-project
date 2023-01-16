using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GPFriendsTaps : MonoBehaviour
{
    public TextMeshProUGUI m_friendCounter;
    public List<Button> m_buttons;

    [Header("Tab Settings")]
    public Transform m_tabFocusImage;
    public Color m_selectedTabColor;
    public Color m_originalTabColor;

    [Header("Audio Settings")]
    public AudioClip m_changeTabSFX;

    // Start is called before the first frame update
    void Start()
    {
        SetFriendCounterText(GPPlayerProfile.m_instance.m_friends.Count);
    }

    public void AddFriendToTeam()
    {
        //TODO: Handle joining team.
    }

    /// <summary>
    /// Displays the friend number on the UI.
    /// </summary>
    /// <param name="friendCount"></param>
    public void SetFriendCounterText(int friendCount)
    {
        m_friendCounter.text = friendCount.ToString();
    }


    /// <summary>
    /// Animates selection tab sprite.
    /// </summary>
    /// <param name="targetTransform"></param>
    public void MoveTapFocus(Transform targetTransform)
    {
        LeanTween.move(m_tabFocusImage.gameObject, targetTransform.position, 0.3f).setEaseSpring();
    }

    /// <summary>
    /// Play SFX and moves the focus sprite to the clicked button.
    /// </summary>
    /// <param name="buttonIdx"></param>
    public void OnButtonClicked(int buttonIdx)
    {
        MoveTapFocus(m_buttons[buttonIdx].transform);
        AudioManager.Instance.Play2D(m_changeTabSFX);
    }


}
