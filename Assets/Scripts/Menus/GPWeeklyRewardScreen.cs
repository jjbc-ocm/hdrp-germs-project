using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GPWeeklyRewardScreen : GPGUIScreen
{
    public TextMeshProUGUI m_rewardCounterText;
    public List<GPWeeklyRewardBlock> m_rewardBlocks;
    [Tooltip("Prizes to give each day in order. Only the first 7 will be used (one per day).")]
    public List<GPPrize> m_prizes;

    [HideInInspector]
    public int m_currClaimedIdx = 0;
    public int m_rewardCount = 7;

    // Start is called before the first frame update
    void Start()
    {

    }

    public override void Show()
    {
        base.Show();
        m_rewardCounterText.text = string.Format("Daily Login Rewards  <color=#f6e19c>{0} </color>/ {1}", m_currClaimedIdx, m_rewardCount);

        for (int i = 0; i < m_rewardBlocks.Count; i++)
        {
            m_rewardBlocks[i].ToggleChecked(i < m_currClaimedIdx);
            m_rewardBlocks[i].ToggleFocus(i == m_currClaimedIdx);
            m_rewardBlocks[i].SetDay(i+1);
        }
    }

    public override void Hide()
    {
        base.Hide();
    }

    public void ClaimReward()
    {
        m_currClaimedIdx++;
        m_currClaimedIdx = Mathf.Clamp(m_currClaimedIdx, 0, 7);
        Hide();
    }

}
