using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingItemInfo
{
    [SerializeField]
    private string name;

    [SerializeField]
    private string[] options;

    public string Name { get => name; }

    public string[] Options { get => options; }
}