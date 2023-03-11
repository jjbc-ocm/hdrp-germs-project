using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingCategoryUI : ListViewUI<SettingItemUI, SettingCategoryUI>
{
    [SerializeField]
    private TMP_Text textName;

    public SettingCategoryInfo Data { get; set; }

    public List<Func<float>> GetOption { get; set; }

    public List<Action<float>> SetOption { get; set; }

    private bool isInitialized;

    protected override void OnRefreshUI()
    {
        textName.text = Data.Name;

        var index = 0;

        RefreshItems(Data.Items, (item, data) =>
        {
            item.Data = data;

            item.GetOption = GetOption[index];

            item.SetOption = SetOption[index];

            index++;
        }, 
        isInitialized);

        isInitialized = true;
    }
}
