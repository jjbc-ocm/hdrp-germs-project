using System.Collections;
using System.Collections.Generic;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuideItemUI : UI<GuideItemUI>
{
    [SerializeField]
    private TMP_Text textTitle;

    [SerializeField]
    private TMP_Text textInfo;

    [SerializeField]
    private Image imageFrame;

    [SerializeField]
    private Image imageBackground;

    [SerializeField]
    private Image imageIcon;

    public GuideData Data { get; set; }

    public void OnClick()
    {
        GameManager.Instance.ui.RemoveGuideItem(Data);
    }

    protected override void OnRefreshUI()
    {
        textTitle.text = Data.Title;

        textInfo.text = Data.Text;

        imageFrame.color = Data.ColorFrame;

        imageBackground.color = Data.ColorBack;

        imageIcon.color = Data.ColorIcon;
    }
}
