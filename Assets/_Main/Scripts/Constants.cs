using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FireType
{
    Straight,
    Reticle,
    Sight
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