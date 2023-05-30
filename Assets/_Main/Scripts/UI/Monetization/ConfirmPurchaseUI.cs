using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPurchaseUI : WindowUI<ConfirmPurchaseUI>
{
    [SerializeField]
    private TMP_Text textHeader;

    [SerializeField]
    private Image imageIcon;

    [SerializeField]
    private TMP_Text textContent;

    public string Header { get; set; }

    public Sprite IconContent { get; set; }

    public string TextContent { get; set; }

    public Action OnConfirm { get; set; }

    public void OnConfirmClick()
    {
        OnConfirm.Invoke();
    }

    protected override void OnRefreshUI()
    {
        textHeader.text = Header;

        imageIcon.sprite = IconContent;

        textContent.text = TextContent;
    }
}
