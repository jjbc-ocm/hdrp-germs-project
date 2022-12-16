using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GPProfileIconBlock : MonoBehaviour
{
    public Image m_image;
    public Button m_button;
    public GPProfileIconSO m_profileIconDesc;
    public UnityEvent<GPProfileIconBlock> onClickedEvent;
    public GameObject m_lockedSprite;
    public bool m_isLocked = false;

    // Start is called before the first frame update
    void Awake()
    {
        m_button.onClick.AddListener(OnClicked);
        if (m_profileIconDesc != null)
        {
            SetProfileIconDesc(m_profileIconDesc);
        }

        ToggleLocked(m_isLocked);
    }

    public void SetProfileIconDesc(GPProfileIconSO desc)
    {
        m_profileIconDesc = desc;
        m_image.sprite = desc.m_sprite;
    }

    public void ToggleLocked(bool locked)
    {
        m_isLocked = locked;
        m_lockedSprite.SetActive(locked);
    }

    void OnClicked()
    {
        if (onClickedEvent != null)
        {
            onClickedEvent.Invoke(this);
        }
    }


}
