using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : WindowUI<LoadingUI>
{
    [SerializeField]
    private TMP_Text textVersion;

    [SerializeField]
    private TMP_Text textMessage;

    [SerializeField]
    private Slider sliderProgress;

    public string Text { get; set; }

    public float Progress { get; set; }

    protected override void OnRefreshUI()
    {
        textVersion.text = Application.version;

        textMessage.text = Text;

        sliderProgress.value = Progress;
    }
}
