using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SOManager : MonoBehaviour
{
    public static SOManager Instance;

    [SerializeField]
    private ConstantsSO constants;

    [SerializeField]
    private SettingsSO settings;

    public ConstantsSO Constants { get => constants; }

    public SettingsSO Settings { get => settings; }


    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }
}
