using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SOManager : Singleton<SOManager>
{
    [SerializeField]
    private ConstantsSO constants;

    [SerializeField]
    private SettingsSO settings;

    [SerializeField]
    private GPShipDesc[] playerShips;

    [SerializeField]
    private GPShipDesc[] botShips;

    public ConstantsSO Constants { get => constants; }

    public SettingsSO Settings { get => settings; }

    public GPShipDesc[] PlayerShips { get => playerShips; }

    public GPShipDesc[] BotShips { get => botShips; }


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
