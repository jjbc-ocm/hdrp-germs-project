using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GPDevFeaturesSettingsSO", menuName = "ScriptableObjects/GPDevFeaturesSettingsSO")]
public class GPDevFeaturesSettingsSO : ScriptableObject
{
    [Header("In progress Features")]
    public bool m_store;
    public bool m_expBar;
    public bool m_friends;

    [Header("Dev cheats")]
    public bool m_levelUpButton;
    public bool m_weeklyRewardButton;
    public bool m_skipPlayerSearch;
}
