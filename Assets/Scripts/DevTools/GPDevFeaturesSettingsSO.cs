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

    [Header("Steam Features")]
    [SerializeField]
    private bool m_LoginAsAnonymous;

    [Header("Unity Gaming Services")]
    [SerializeField]
    [Tooltip("It can be \"dev\" or \"production\".")]
    private string m_Environment;

    public bool LoginAsAnonymous { get => m_LoginAsAnonymous; }

    public string Environment { get => m_Environment; }
}
