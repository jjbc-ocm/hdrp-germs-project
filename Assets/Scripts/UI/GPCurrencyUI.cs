using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GPCurrencyUI : MonoBehaviour
{
    public TextMeshProUGUI m_goldAmountText;
    public TextMeshProUGUI m_gemAmountText;

    // Start is called before the first frame update
    void Start()
    {
        GPPlayerProfile.m_instance.OnGoldModifiedEvent.AddListener(UpdateGoldUI);
        GPPlayerProfile.m_instance.OnGemsModifiedEvent.AddListener(UpdateGemsUI);

        UpdateGoldUI();
        UpdateGemsUI();
    }

    public void UpdateGoldUI()
    {
        m_goldAmountText.text = GPPlayerProfile.m_instance.m_gold.ToString();
    }

    public void UpdateGemsUI()
    {
        m_gemAmountText.text = GPPlayerProfile.m_instance.m_gems.ToString();
    }
}
