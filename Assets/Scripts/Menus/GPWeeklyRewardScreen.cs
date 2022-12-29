using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GPWeeklyRewardScreen : GPGUIScreen
{
    public TextMeshProUGUI m_rewardCounterText;

    // Start is called before the first frame update
    void Start()
    {

    }

    public override void Show()
    {
        base.Show();
    }

    public override void Hide()
    {
        base.Hide();
    }

    public void ClaimReward()
    {
        Hide();
    }

}
