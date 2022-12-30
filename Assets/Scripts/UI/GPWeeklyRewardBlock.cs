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

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetRewardSprite(Sprite sprite)
    {
        m_rewardImage.sprite = sprite;
    }

    public void SetRewardAmount(int amount)
    {
        m_rewardAmountText.text = amount.ToString();
    }

    public void SetDay(int day)
    {
        m_dayText.text = "DAY" + day.ToString();
    }

    public void ToggleChecked(bool active)
    {
        m_checkObj.SetActive(active);
    }

    public void ToggleFocus(bool active)
    {
        m_focusObj.SetActive(active);
    }
}
