using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class GPAddFriendSlot : MonoBehaviour
{
    [Header("UI references")]
    public TextMeshProUGUI m_friendName;
    public GPUserFrameUI m_userMinitaureUI;

    [Header("Events")]
    public UnityEvent<GPFriend> onAddButtonClickedEvent;

    [HideInInspector]
    public GPFriend m_asignedUser;

    /// <summary>
    /// Displays the data of the assigned user.
    /// </summary>
    /// <param name="user"></param>
    public void AssignUserAndDisplay(GPFriend user)
    {
        m_asignedUser = user;
        SetFriendName(user.m_friendName);
        SetFriendProfileIcon(user.m_profileIcon);
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
    /// Linked to the button component of the plus icon.
    /// </summary>
    public void OnAddFriendButtonClicked()
    {
        if (onAddButtonClickedEvent != null)
        {
            onAddButtonClickedEvent.Invoke(m_asignedUser);
        }
    }
}
