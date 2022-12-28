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
    public TextMeshProUGUI m_friendName;
    public TextMeshProUGUI m_friendOnlineStatus;
    public List<Image> m_profileIcons;
    public Image m_statusIcon;
    public Sprite m_offlineSprite;
    public Sprite m_onlineSprite;
    public Button m_addButton;
    public Button m_chatButton;
    public Button m_blockButton;

    public UnityEvent<GPFriend> onAddButtonClickedEvent;
    public UnityEvent<GPFriend> onChatButtonClickedEvent;
    public UnityEvent<GPFriend> onBlockButtonClickedEvent;

    public GPFriend m_asignedFriend;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void AssignToFriendAndDisplay(GPFriend friend)
    {
        m_asignedFriend = friend;
        SetFriendName(friend.m_friendName);
        SetOnlineStatus(friend.m_onlineStatus);
        SetFriendProfileIcon(friend.m_profileIcon);
    }

    public void SetFriendName(string name)
    {
        m_friendName.text = name;
    }

    public void SetFriendProfileIcon(Sprite sprite)
    {
        foreach (var icon in m_profileIcons)
        {
            icon.sprite = sprite;
        }
    }

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

    public void OnAddButtonClicked()
    {
        if (onAddButtonClickedEvent != null)
        {
            onAddButtonClickedEvent.Invoke(m_asignedFriend);
        }
    }

    public void OnChatButtonClicked()
    {
        if (onChatButtonClickedEvent != null)
        {
            onChatButtonClickedEvent.Invoke(m_asignedFriend);
        }
    }

    public void OnBlockButtonClicked()
    {
        if (onBlockButtonClickedEvent != null)
        {
            onBlockButtonClickedEvent.Invoke(m_asignedFriend);
        }
    }

}
