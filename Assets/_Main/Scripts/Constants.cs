using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    public const string ENV_NAME = "dev"; // "production" or "dev"

    public const string NETWORK_VERSION = "1";

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

    public const float GAME_MAX_TIMER = 600;

    public const int FOG_OF_WAR_DISTANCE = 150;

    public const float MOVE_SPEED_TO_SECONDS_RATIO = 25f;

    public const int SCORE_REQUIRED = 50;

    public const float RESPAWN_TIME = 10;

    public static float[] SETTINGS_RENDER_SCALE = new float[] { 1f, 0.9f, 0.8f, 0.7f, 0.6f, 0.5f };

    public static int[] SETTINGS_MSAA = new int[] { 8, 0 };

    public static bool[] SETTINGS_POST_PROCESS = new bool[] { true, false };
}

public class Globals
{
    public static string ROOM_NAME;
}

public enum AimType 
{ 
    None, /* For skills that target your own ship */
    WhileExecute, /* For skills like normal attack */
    Water, /* For skills that has area of effect */
    EnemyShip, /* For skills that always hit the enemy */
    AllyShip, /* For skills that always hit the ally */
    AnyShip /* For skills that always hit either enemy or ally */
}

public enum TargetType
{
    One,
    Area
}

public enum CategoryType
{
    Consumables,
    Offensive,
    Defensive,
    Utility,
}

public enum StatFilterType
{
    All,
    AttackDamage,
    AbilityPower,
    AttackSpeed,
    Armor,
    Resist,
    MovementSpeed,
    Health,
    Mana,
    Cooldown,
    LifeSteal
}

public enum BattleResultType
{
    Draw,
    Victory,
    Defeat
}