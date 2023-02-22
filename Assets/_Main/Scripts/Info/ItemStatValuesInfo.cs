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

    /*public StatsInfo(Player player)
    {
        health = player.Stat.MaxHealth();

        mana = player.Stat.MaxMana();

        attackDamage = player.Stat.AttackDamage();

        abilityPower = player.Stat.AbilityPower();

        armor = player.Stat.Armor();

        resist = player.Stat.Resist();

        attackSpeed = player.Stat.AttackSpeed();

        moveSpeed = player.Stat.MoveSpeed();
    }

    public StatsInfo(Player player, ItemData item)
    {
        health = player.Stat.MaxHealth(item.StatModifier.BuffMaxHealth);

        mana = player.Stat.MaxMana(item.StatModifier.BuffMaxMana);

        attackDamage = player.Stat.AttackDamage(item.StatModifier.BuffAttackDamage);

        abilityPower = player.Stat.AbilityPower(item.StatModifier.BuffAbilityPower);

        armor = player.Stat.Armor(item.StatModifier.BuffArmor);

        resist = player.Stat.Resist(item.StatModifier.BuffResist);

        attackSpeed = player.Stat.AttackSpeed(item.StatModifier.BuffAttackSpeed);

        moveSpeed = player.Stat.MoveSpeed(item.StatModifier.BuffMoveSpeed);
    }*/

    /*public static StatsInfo operator -(StatsInfo a, StatsInfo b)
    {
        return new StatModifier
        {
            *//*IsInvisible = a.IsInvisible || b.IsInvisible,
            LifeSteal = a.LifeSteal + b.LifeSteal,
            BuffMaxHealth = a.BuffMaxHealth + b.BuffMaxHealth,
            BuffMaxMana = a.BuffMaxMana + b.BuffMaxMana,
            BuffAttackDamage = a.BuffAttackDamage + b.BuffAttackDamage,
            BuffAbilityPower = a.BuffAbilityPower + b.BuffAbilityPower,
            BuffArmor = a.BuffArmor + b.BuffArmor,
            BuffResist = a.BuffResist + b.BuffResist,
            BuffAttackSpeed = a.BuffAttackSpeed + b.BuffAttackSpeed,
            BuffMoveSpeed = a.BuffMoveSpeed + b.BuffMoveSpeed,
            BuffCooldown = a.BuffCooldown + b.BuffCooldown*//*
        };
    }*/
}
