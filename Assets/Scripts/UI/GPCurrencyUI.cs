using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GPCurrencyUI : UI<GPCurrencyUI>
{
    public static GPCurrencyUI Instance;

    public TextMeshProUGUI m_goldAmountText;
    public TextMeshProUGUI m_gemAmountText;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        RefreshUI();
    }

    protected override void OnRefreshUI()
    {
        var data = APIManager.Instance.PlayerData;

        m_goldAmountText.text = data.Coins.ToString();

        m_gemAmountText.text = data.Gems.ToString();
    }
}
