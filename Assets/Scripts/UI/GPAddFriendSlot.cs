using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class GPAddFriendSlot : MonoBehaviour
{
    public TextMeshProUGUI m_friendName;
    public List<Image> m_profileIcons;

    public GPFriend m_asignedUser;
    public UnityEvent<GPFriend> onAddButtonClickedEvent;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void AssignUserAndDisplay(GPFriend user)
    {
        m_asignedUser = user;
        SetFriendName(user.m_friendName);
        SetFriendProfileIcon(user.m_profileIcon);
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

    public void OnAddFriendButtonClicked()
    {
        if (onAddButtonClickedEvent != null)
        {
            onAddButtonClickedEvent.Invoke(m_asignedUser);
        }
    }
}
