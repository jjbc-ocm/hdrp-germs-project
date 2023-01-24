using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingCategoryInfo
{
    [SerializeField]
    private string name;

    [SerializeField]
    private SettingItemInfo[] items;

    public string Name { get => name; }

    public SettingItemInfo[] Items { get => items; }
}