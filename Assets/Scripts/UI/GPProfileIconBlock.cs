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

    [Header("Audio settings")]
    public AudioClip m_clickSFX;

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

    /// <summary>
    /// Displays the profile icon sprite from the given GPProfileIconSO.
    /// </summary>
    /// <param name="desc"></param>
    public void SetProfileIconDesc(GPProfileIconSO desc)
    {
        m_profileIconDesc = desc;
        m_image.sprite = desc.m_sprite;
    }

    /// <summary>
    /// If true, shows the prifile icon as locked.
    /// </summary>
    /// <param name="locked"></param>
    public void ToggleLocked(bool locked)
    {
        m_isLocked = locked;
        m_lockedSprite.SetActive(locked);
    }

    /// <summary>
    /// Linked to the OnClick event of the button component.
    /// Plays SFX and calls onClickedEvent to handle the equip from somewhere else.
    /// </summary>
    void OnClicked()
    {
        AudioManager.Instance.Play2D(m_clickSFX);
        if (onClickedEvent != null)
        {
            onClickedEvent.Invoke(this);
        }
    }


}
