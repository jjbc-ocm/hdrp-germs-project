using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dummy Pirates/Constants")]
public class ConstantsSO : ScriptableObject
{
    [Header("Network")]

    [SerializeField]
    private string networkVersion = "0.6";

    public string NetworkVersion { get => networkVersion; }

    [Header("Layers")]

    [SerializeField]
    private string layerWater = "Water";

    [SerializeField]
    private string layerEnvironment = "Environment";

    [SerializeField]
    private string layerMinimap = "Minimap";

    [SerializeField]
    private string layerAlly = "Ally Ship";

    [SerializeField]
    private string layerEnemy = "Enemy Ship";

    [SerializeField]
    private string layerMonster = "Monster";

    [SerializeField]
    private string layerBullet = "Bullet";

    [SerializeField]
    private string layerItem = "Item";

    [SerializeField]
    private string layerBush = "Bush";

    public string LayerWater { get => layerWater; }

    public string LayerEnvironment { get => layerEnvironment; }

    public string LayerMinimap { get => layerMinimap; }

    public string LayerAlly { get => layerAlly; }

    public string LayerEnemy { get => layerEnemy; }

    public string LayerMonster { get => layerMonster; }

    public string LayerBullet { get => layerBullet; }

    public string LayerItem { get => layerItem; }

    public string LayerBush { get => layerBush; }





    public const string DEBUG_SCENE_NAME = "Debug";

    public const string MENU_SCENE_NAME = "Intro";

    public const string WAITING_ROOM_SCENE_NAME = "Waiting_Room";

    public const string GAME_SCENE_NAME = "CTF_Game";

    public const string KEY_TEAM = "team";

    public const string KEY_SHIP_INDEX = "shipIndex";

    public const int MAX_PLAYER_COUNT = 6;

    public const int MIN_PLAYER_COUNT = 6;

    public const int MAX_TEAM = 2;

    public const int MAX_PLAYER_COUNT_PER_TEAM = MAX_PLAYER_COUNT / MAX_TEAM;

    public const float GAME_MAX_TIMER = 1200;

    public const int FOG_OF_WAR_DISTANCE = 150;

    public const float MOVE_SPEED_TO_SECONDS_RATIO = 25f;

    public const int SCORE_REQUIRED = 50;

    public const float RESPAWN_TIME = 10;

    public const int LAYER_ALLY = 8;

    public const int LAYER_ENEMY = 13;

    [Header("Settings")]

    [SerializeField]
    private Vector2Int[] settingResolutions = new Vector2Int[]
    {
        new Vector2Int(3840, 2160),
        new Vector2Int(2560, 1440),
        new Vector2Int(1920, 1080),
        new Vector2Int(1280, 720)
    };

    [SerializeField]
    private float[] settingRenderScales = new float[] { 1f, 0.9f, 0.8f, 0.7f, 0.6f, 0.5f };

    [SerializeField]
    private int[] settingMSAA = new int[] { 8, 0 };

    [SerializeField]
    private bool[] settingPostProcessing = new bool[] { true, false };

    public Vector2Int[] SettingResolutions { get => settingResolutions; }

    public float[] SettingRenderScales { get => settingRenderScales; }

    public int[] SettingMSAA { get => settingMSAA; }

    public bool[] SettingPostProcessing { get => settingPostProcessing; }
    
}
