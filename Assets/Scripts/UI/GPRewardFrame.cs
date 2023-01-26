using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GPRewardFrame : MonoBehaviour
{
    public Image m_rewardImage;
    public Image m_rewardFrame;
    public TextMeshProUGUI m_rewardAmount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// Displays the reward icon sprite, reward amount 
    /// and reward back frame of the given GPRewardSO.
    /// </summary>
    /// <param name="rewardSO"></param>
    /// <param name="amount"></param>
    public void DisplayReward(GPRewardSO rewardSO, int amount)
    {
        m_rewardImage.sprite = rewardSO.m_rewardSprite;
        m_rewardFrame.sprite = rewardSO.m_rewardFrame;
        m_rewardAmount.text = amount.ToString();
    }

    
}
