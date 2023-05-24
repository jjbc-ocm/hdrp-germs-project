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

    [SerializeField]
    private GameObject selectedIndicator;

    public DummyPartSO Data { get; set; }

    public bool IsSelected { get; set; }

    public Action OnClickCallback { get; set; }

    public void OnClick()
    {
        OnClickCallback.Invoke();
    }

    protected override void OnRefreshUI()
    {
        imageIcon.sprite = Data.Icon;

        textCost.text = Data.Cost.ToString();

        selectedIndicator.SetActive(IsSelected);
    }
}
