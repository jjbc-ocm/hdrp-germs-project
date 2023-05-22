using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DPStoreItemUI : UI<DPStoreItemUI>
{
    [SerializeField]
    private Image imageIcon;

    [SerializeField]
    private TMP_Text textCost;

    public DummyPartSO Data { get; set; }

    public Action OnClickCallback { get; set; }

    public void OnClick()
    {
        OnClickCallback.Invoke();
    }

    protected override void OnRefreshUI()
    {

    }
}
