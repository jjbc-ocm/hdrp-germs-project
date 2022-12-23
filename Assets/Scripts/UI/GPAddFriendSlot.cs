using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GPAddFriendSlot : MonoBehaviour
{
    public TextMeshProUGUI m_friendName;
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
}
