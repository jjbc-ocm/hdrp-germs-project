using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    // Start is called before the first frame update
    void Start()
    {

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
                break;
            case GP_ONLINE_STATUS.kOnline:
                m_friendOnlineStatus.text = "Online";
                break;
            default:
                m_friendOnlineStatus.text = "Unknow";
                break;
        }
    }

}
