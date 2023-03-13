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


    [Header("Scenes")]

    [SerializeField]
    private string sceneDebug = "Debug";

    [SerializeField]
    private string sceneMenu = "Intro";

    [SerializeField]
    private string scenePreparation = "Preparation";

    [SerializeField]
    private string sceneGame = "CTF_Game";

    public string SceneDebug { get => sceneDebug; }

    public string SceneMenu { get => sceneMenu; }

    public string ScenePreparation { get => scenePreparation; }

    public string SceneGame { get => sceneGame; }


    [Header("In-Game")]

    [SerializeField]
    private double preparationTime = 30f;

    [SerializeField]
    private float captureChestTime = 45f;

    [SerializeField]
    private byte maxPlayerCount = 6;

    [SerializeField]
    private int maxTeam = 2;

    [SerializeField]
    private int maxPlayerPerTeam = 3;

    [SerializeField]
    private float gameTimer = 1200f;

    [SerializeField]
    private float fogOrWarDistance = 150f;

    [SerializeField]
    private float moveSpeedToSecondsRatio = 25f;

    [SerializeField]
    private int scoreRequired = 50;

    [SerializeField]
    private float respawnTime = 10f;

    public double PreparationTime { get => preparationTime; }

    public float CaptureChestTime { get => captureChestTime; }

    public byte MaxPlayerCount { get => maxPlayerCount; }

    public int MaxTeam { get => maxTeam; }

    public int MaxPlayerPerTeam { get => maxPlayerPerTeam; }

    public float GameTimer { get => gameTimer; }

    public float FogOrWarDistance { get => fogOrWarDistance; }

    public float MoveSpeedToSecondsRatio { get => moveSpeedToSecondsRatio; }

    public int ScoreRequired { get => scoreRequired; }

    public float RespawnTime { get => respawnTime; }


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
