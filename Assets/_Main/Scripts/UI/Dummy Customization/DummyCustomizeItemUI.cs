using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DummyCustomizeItemUI : UI<DummyCustomizeItemUI>
{
    [SerializeField]
    private Image imageIcon;

    [SerializeField]
    private GameObject imageIndicator;

    public DummyPartSO Data { get; set; }

    public Action OnClickCallback { get; set; }

    public void OnClick()
    {
        OnClickCallback.Invoke();
    }

    protected override void OnRefreshUI()
    {
        imageIcon.sprite = Data.Icon;
    }
}
