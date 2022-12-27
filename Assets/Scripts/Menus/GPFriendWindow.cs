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

    [Header("Friend List Settings")]
    public Transform m_friendsHolder;
    public GPFriendSlot m_friendSlotPrefab;
    public TMP_InputField m_searchFriendInputField;
    public Vector3 m_slotLocalEulerAngles = new Vector3(0, 0, 180);
    public Vector3 m_slotPivot = new Vector2(0.675f, 0.29f);

    [Header("Add Friend List Settings")]
    public Transform m_addFriendsHolder;
    public GPAddFriendSlot m_addFriendSlotPrefab;
    public TMP_InputField m_searchUserInputField;

    [Header("Tab Settings")]
    public Transform m_tabFocusImage;
    public Color m_selectedTabColor;
    public Color m_originalTabColor;

    GPGUIScreen m_currentScreen = null;

    [Header("Audio Settings")]
    public AudioClip m_changeTabSFX;

    [Header("Testing Settings")]
    public List<GPFriend> m_testFriends = new List<GPFriend>();
    public bool m_useTestFriends = false;

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

#if !UNITY_EDITOR
        m_useTestFriends = false;
#endif
        if (m_useTestFriends)
        {
            UpdateFriendList(m_testFriends);
        }
        else
        {
            UpdateFriendList(GPPlayerProfile.m_instance.m_friends);
        }
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

    public void UpdateFriendList(List<GPFriend> friends)
    {
        //clear old UI
        foreach (Transform child in m_friendsHolder)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (var friend in friends)
        {
            GPFriendSlot slot = Instantiate(m_friendSlotPrefab, m_friendsHolder);
            slot.transform.localEulerAngles = m_slotLocalEulerAngles;
            slot.GetComponent<RectTransform>().pivot = m_slotPivot;
            slot.SetFriendName(friend.m_friendName);
            slot.SetOnlineStatus(friend.m_onlineStatus);
            slot.SetFriendProfileIcon(friend.m_profileIcon);
        }
    }

    public void UpdateAddFriendList(List<GPFriend> results)
    {
        //clear old UI
        foreach (Transform child in m_addFriendsHolder)
        {
            GameObject.Destroy(child.gameObject);
        }

        //Fill new UI
        foreach (var friend in results)
        {
            GPAddFriendSlot slot = Instantiate(m_addFriendSlotPrefab, m_addFriendsHolder);
            slot.transform.localEulerAngles = m_slotLocalEulerAngles;
            slot.GetComponent<RectTransform>().pivot = m_slotPivot;
            slot.SetFriendName(friend.m_friendName);
            slot.SetFriendProfileIcon(friend.m_profileIcon);
        }
    }

}
