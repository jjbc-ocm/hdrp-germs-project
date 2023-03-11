using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingItemUI : UI<SettingItemUI>
{
    [SerializeField]
    private TMP_Text textName;

    [SerializeField]
    private TMP_Text textOption;

    [SerializeField]
    private Slider sliderOption;

    public SettingItemInfo Data { get; set; }

    public Func<float> GetOption { get; set; }

    public Action<float> SetOption { get; set; }

    public void OnClick()
    {
        if (Data.Options.Length == 0) return;

        SetOption.Invoke(0);
    }

    public void OnSliderChanged(float value)
    {
        SetOption.Invoke(value);
    }

    protected override void OnRefreshUI()
    {
        var isTextOption = Data.Options.Length > 0;

        textName.text = Data.Name;

        textOption.gameObject.SetActive(isTextOption);

        sliderOption.gameObject.SetActive(!isTextOption);

        if (isTextOption)
        {
            textOption.text = Data.Options[(int)GetOption.Invoke() % Data.Options.Length];
        }
        else
        {
            sliderOption.value = GetOption.Invoke();
        }
    }
}
