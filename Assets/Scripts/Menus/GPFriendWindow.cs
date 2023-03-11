using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GPFriendWindow : GPGWindowUI
{
    [Header("Screen References")]
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
    public List<GPFriend> m_possibleFriends = new List<GPFriend>();
    public bool m_useTestFriends = false;

    [Header("Click outside to hide settings")]
    public Camera m_camera;
    public List<GameObject> m_boundaryPanels;

    // Start is called before the first frame update
    void Start()
    {
        m_friendsTabButton.onClick.AddListener(delegate { ShowFriendScreen(true); });
        m_addFriendTabButton.onClick.AddListener(delegate { ShowAddFriendScreen(true); });
        m_currentScreen = m_friendScreen;
        ShowFriendScreen(false);

        m_searchFriendInputField.onEndEdit.AddListener(OnSearchBoxEndEdit);
    }

    private void Update()
    {
        if (InputManager.Instance.IsAttack)
        {
            int outsideCount = 0;
            foreach (var bound in m_boundaryPanels)
            {
                if (bound.activeInHierarchy &&
                    !RectTransformUtility.RectangleContainsScreenPoint(
                        bound.GetComponent<RectTransform>(),
                        InputManager.Instance.Look,
                        m_camera))
                {
                    outsideCount++;
                }
            }

            if (outsideCount >= m_boundaryPanels.Count)
            {
                Hide();
            }
        }
        
    }


    /// <summary>
    /// Displays the friend list and moves the tap focus sprite.
    /// </summary>
    /// <param name="playSFX"></param>
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
            UpdateFriendListUI(m_testFriends);
        }
        else
        {
            UpdateFriendListUI(GPPlayerProfile.m_instance.m_friends);
        }
    }

    /// <summary>
    /// Displays the add friend list and moves the tap focus sprite.
    /// </summary>
    /// <param name="playSFX"></param>
    public void ShowAddFriendScreen(bool playSFX)
    {
        m_currentScreen.Hide();
        m_addFriendScreen.Show();

        m_friendsTabButton.GetComponent<TextMeshProUGUI>().color = m_originalTabColor;

        m_currentScreen = m_addFriendScreen;

        MoveTapFocus(m_addFriendTabButton.transform);
        m_addFriendTabButton.GetComponent<TextMeshProUGUI>().color = m_selectedTabColor;
        OnNewTabShown(playSFX);

        //only show users if there is sometihng written on the search box
        if (m_searchFriendInputField.text != "")
        {
            OnSearchBoxEndEdit(m_searchFriendInputField.text);
        }
        else // if there is nothing written then clear the UI list.
        {
            UpdateAddFriendListUI(null);
        }
    }

    /// <summary>
    /// Logic to do when a new tab is shown.
    /// </summary>
    void OnNewTabShown(bool playSFX)
    {
        if (playSFX)
        {
            AudioManager.Instance.Play2D(m_changeTabSFX);
        }

        m_searchFriendInputField.text = "";
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
    /// Instantiates a friend UI slot for each friend and fills it's content.
    /// </summary>
    /// <param name="friends"></param>
    public void UpdateFriendListUI(List<GPFriend> friends)
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
            slot.AssignToFriendAndDisplay(friend);
        }
    }

    /// <summary>
    /// Instantiates a user UI slot for each user found and fills it's content.
    /// </summary>
    /// <param name="results"></param>
    public void UpdateAddFriendListUI(List<GPFriend> results)
    {
        //clear old UI
        foreach (Transform child in m_addFriendsHolder)
        {
            GameObject.Destroy(child.gameObject);
        }

        if (results == null) { return; }

        //Fill new UI
        foreach (var friend in results)
        {
            GPAddFriendSlot slot = Instantiate(m_addFriendSlotPrefab, m_addFriendsHolder);
            slot.transform.localEulerAngles = m_slotLocalEulerAngles;
            slot.GetComponent<RectTransform>().pivot = m_slotPivot;
            slot.AssignUserAndDisplay(friend);
        }
    }

    public List<GPFriend> FilterUsers(List<GPFriend> friends)
    {
        return friends.FindAll(FindFriend);
    }

    private bool FindFriend(GPFriend friend)
    {

        if (friend.m_friendName.Contains(m_searchFriendInputField.text))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Filters the friend list based on the what's written on the search text box.
    /// </summary>
    /// <param name="text"></param>
    public void OnSearchBoxEndEdit(string text)
    {
        if (m_useTestFriends)
        {
            if (m_currentScreen == m_friendScreen)
            {
                UpdateFriendListUI(FilterUsers(m_testFriends));
            }
            else if (m_currentScreen == m_addFriendScreen)
            {
                UpdateAddFriendListUI(FilterUsers(m_possibleFriends));
            }
        }
        else
        {
            if (m_currentScreen == m_friendScreen)
            {
                UpdateFriendListUI(FilterUsers(GPPlayerProfile.m_instance.m_friends));
            }
            else if (m_currentScreen == m_addFriendScreen)
            {
                // TODO: We should probably do the user filtering from the API and return them to just display them on the UI.
                //UpdateAddFriendList(FilterUsers(putListOfUsersHere)); 
            }
        }
        
    }

}
