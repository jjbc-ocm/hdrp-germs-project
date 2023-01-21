using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum GP_PROFILE_FRAME
{
    kWooden,
    kSilver,
    kGolden,
    kCrystal
}

[System.Serializable]
public class GPUserFrames
{
    public GP_PROFILE_FRAME m_frameType;
    public Image m_profileIconImage;
    public TextMeshProUGUI m_levelText;
    public GameObject m_holder;
}

public class GPUserFrameUI : MonoBehaviour
{
    public List<GPUserFrames> m_frames;
    [HideInInspector]
    public Sprite m_assignedProfileIconSprite;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// Sets the displayed level on the user frame.
    /// </summary>
    /// <param name="lvl"></param>
    public void SetLevel(int lvl)
    {
        foreach (var frame in m_frames)
        {
            frame.m_levelText.text = lvl.ToString();
        }
    }

    /// <summary>
    /// Sets the displayed profile icon sprite of the user
    /// </summary>
    /// <param name="icon"></param>
    public void SetProfileIcon(Sprite icon)
    {
        m_assignedProfileIconSprite = icon;
        foreach (var frame in m_frames)
        {
            frame.m_profileIconImage.sprite = icon;
        }
    }

    /// <summary>
    /// Sets the rank frame of the user.
    /// </summary>
    /// <param name="frameType"></param>
    public void SetFrame(GP_PROFILE_FRAME frameType)
    {
        foreach (var frame in m_frames)
        {
            frame.m_holder.SetActive(frame.m_frameType == frameType);
        }
    }
}
