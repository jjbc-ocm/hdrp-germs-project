using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GPWeeklyRewardBlock : MonoBehaviour
{
    public Image m_rewardImage;
    public TextMeshProUGUI m_rewardAmountText;
    public TextMeshProUGUI m_dayText;
    public GameObject m_checkObj;
    public GameObject m_focusObj;
    public string m_focusText = "TODAY";
    public Color m_focusColor;
    public Color m_normalColor;

    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    /// Sets the displayed reward sprite.
    /// </summary>
    /// <param name="sprite"></param>
    public void SetRewardSprite(Sprite sprite)
    {
        m_rewardImage.sprite = sprite;
    }

    /// <summary>
    /// Sets the displayed reward amount.
    /// </summary>
    /// <param name="amount"></param>
    public void SetRewardAmount(int amount)
    {
        m_rewardAmountText.text = amount.ToString();
    }

    /// <summary>
    /// Sets the displayed day of the reward.
    /// </summary>
    /// <param name="day"></param>
    public void SetDay(int day)
    {
        m_dayText.text = "DAY " + day.ToString();
    }

    /// <summary>
    /// Toggle the checkmark sprite.
    /// </summary>
    /// <param name="active"></param>
    public void ToggleChecked(bool active)
    {
        m_checkObj.SetActive(active);
    }

    /// <summary>
    /// Toggle the focus on the reward block to let the user know this is the current reward.
    /// </summary>
    /// <param name="active"></param>
    public void ToggleFocus(bool active)
    {
        m_focusObj.SetActive(active);

        if (active)
        {
            m_dayText.text = m_focusText;
            m_dayText.color = m_focusColor;
        }
        else
        {
            m_dayText.color = m_normalColor;
        }

    }
}
