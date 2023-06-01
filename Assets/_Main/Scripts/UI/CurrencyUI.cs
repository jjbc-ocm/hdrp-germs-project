using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyUI : UI<CurrencyUI>
{
    [SerializeField]
    private TMP_Text textGems;

    public long Gems { get; set; }

    protected override void OnRefreshUI()
    {
        textGems.text = Gems.ToString();
    }
}
