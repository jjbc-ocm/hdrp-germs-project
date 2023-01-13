using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public enum GP_ONLINE_STATUS
{
    kOffline,
    kOnline,
}

public class GPFriendSlot : MonoBehaviour
{
    [Header("UI references")]
    public TextMeshProUGUI m_friendName;
    public TextMeshProUGUI m_friendOnlineStatus;
    public GPUserFrameUI m_userMinitaureUI;
    public Image m_statusIcon;
    public Sprite m_offlineSprite;
    public Sprite m_onlineSprite;
    public Button m_addButton;
    public Button m_chatButton;
    public Button m_blockButton;

    [Header("Events")]
    //Suscribe to this events for implementation of adding friends, chatting and blocking.
    public UnityEvent<GPFriend> onAddButtonClickedEvent;
    public UnityEvent<GPFriend> onChatButtonClickedEvent;
    public UnityEvent<GPFriend> onBlockButtonClickedEvent;

    [HideInInspector]
    public GPFriend m_asignedFriend;

    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    /// Displays the data of the assigned friend.
    /// </summary>
    /// <param name="user"></param>
    public void AssignToFriendAndDisplay(GPFriend friend)
    {
        m_asignedFriend = friend;
        SetFriendName(friend.m_friendName);
        SetOnlineStatus(friend.m_onlineStatus);
        SetFriendProfileIcon(friend.m_profileIcon);
    }

    /// <summary>
    /// Sets the displayed name.
    /// </summary>
    /// <param name="name"></param>
    public void SetFriendName(string name)
    {
        m_friendName.text = name;
    }

    /// <summary>
    /// Sets the displayed profile icon.
    /// </summary>
    /// <param name="sprite"></param>
    public void SetFriendProfileIcon(Sprite sprite)
    {
        m_userMinitaureUI.SetProfileIcon(sprite);
    }

    /// <summary>
    /// Displays the online status of the friend.
    /// </summary>
    /// <param name="status"></param>
    public void SetOnlineStatus(GP_ONLINE_STATUS status)
    {
        switch (status)
        {
            case GP_ONLINE_STATUS.kOffline:
                m_friendOnlineStatus.text = "Offline";
                m_statusIcon.sprite = m_offlineSprite;
                break;
            case GP_ONLINE_STATUS.kOnline:
                m_friendOnlineStatus.text = "Online";
                m_statusIcon.sprite = m_onlineSprite;
                break;
            default:
                m_friendOnlineStatus.text = "Unknow";
                break;
        }
    }

    /// <summary>
    /// Linked to the onClick event of the button component of the plus icon.
    /// </summary>
    public void OnAddButtonClicked()
    {
        if (onAddButtonClickedEvent != null)
        {
            onAddButtonClickedEvent.Invoke(m_asignedFriend);
        }
    }

    /// <summary>
    /// Linked to the onClick event of the button component of the chat icon.
    /// </summary>
    public void OnChatButtonClicked()
    {
        if (onChatButtonClickedEvent != null)
        {
            onChatButtonClickedEvent.Invoke(m_asignedFriend);
        }
    }

    /// <summary>
    /// Linked to the onClick event of the button component of the block icon.
    /// </summary>
    public void OnBlockButtonClicked()
    {
        if (onBlockButtonClickedEvent != null)
        {
            onBlockButtonClickedEvent.Invoke(m_asignedFriend);
        }
    }

}
