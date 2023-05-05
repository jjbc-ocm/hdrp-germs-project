using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FireType : byte
{
    Straight,
    Reticle,
    Sight
}

public enum AimType : byte
{ 
    None, /* For skills that target your own ship */
    WhileExecute, /* For skills like normal attack */
    Water, /* For skills that has area of effect */
    EnemyShip, /* For skills that always hit the enemy */
    AllyShip, /* For skills that always hit the ally */
    AnyShip /* For skills that always hit either enemy or ally */
}

public enum TargetType : byte
{
    One,
    Area
}

public enum CategoryType : byte
{
    Consumables,
    Offensive,
    Defensive,
    Utility,
}

public enum StatFilterType : byte
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

public enum BattleResultType : byte
{
    Draw,
    Victory,
    Defeat
}
public enum BotTargetType : byte
{
    /// <summary>
    /// This bot is refering to itself
    /// </summary>
    Self,

    /// <summary>
    /// This bot is refering to the target
    /// </summary>
    Target
}

public enum BotPropertyType : byte
{
    /// <summary>
    /// Nearer: 0, Further: 1, Max Cap: FOV distance
    /// </summary>
    Distance,

    /// <summary>
    /// None: 0, Has: 1
    /// </summary>
    Chest,

    /// <summary>
    /// Less Health: 0, Move Health: 1
    /// </summary>
    HealthRatio,

    /// <summary>
    /// Nearer: 0, Further: 1, Max Cap: 1000
    /// </summary>
    DistanceNoFOV,

    /// <summary>
    /// None: 0, Has: 1
    /// </summary>
    Key,

    ManaRatio,

    AttackDamageRatio,

    AbilityPowerRatio,

    ArmorRatio,

    ResistRatio,

    AttackSpeedRatio,

    MoveSpeedRatio,

    LifeStealRatio,

    CooldownRatio,

    InvisibilityRatio,

    CostRatio,

    ConsumableRatio
}

public enum MessageBroadcastType : byte
{
    KeyObtained,

    ChestObtained,

    KeyDropped,

    ChestDropped
}

public enum GameMode
{
    Standard,

    Tutorial
}

public enum TutorialEventTriggerType
{
    OnTriggerEnter,

    OnDestroy,

    OnWaitASecond
}