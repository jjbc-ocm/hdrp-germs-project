using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public struct ItemStatValuesInfo
{
    private float health;
    private float mana;
    private float attackDamage;
    private float abilityPower;
    private float armor;
    private float resist;
    private float attackSpeed;
    private float moveSpeed;
    private float lifeSteal;
    private float cooldown;
    private float cost;

    public float Health { get => health; set => health = value; }

    public float Mana { get => mana; set => mana = value; }

    public float AttackDamage { get => attackDamage; set => attackDamage = value; }

    public float AbilityPower { get => abilityPower; set => abilityPower = value; }

    public float Armor { get => armor; set => armor = value; }

    public float Resist { get => resist; set => resist = value; }

    public float AttackSpeed { get => attackSpeed; set => attackSpeed = value; }

    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

    public float LifeSteal { get => lifeSteal; set => lifeSteal = value; }

    public float Cooldown { get => cooldown; set => cooldown = value; }

    public float Cost { get => cost; set => cost = value; }
}
