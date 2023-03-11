using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dummy Pirates/Settings")]
public class SettingsSO : ScriptableObject
{
    [SerializeField]
    private SettingCategoryInfo[] categories;

    public SettingCategoryInfo[] Categories { get => categories; }
}
