using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatModifierData
{
    /*[SerializeField]
    private int id;*/

    [SerializeField]
    private bool isInvisible;

    [SerializeField]
    private float lifeSteal;

    [SerializeField]
    private float buffMaxHealth;

    [SerializeField]
    private float buffMaxMana;

    [SerializeField]
    private float buffAttackDamage;

    [SerializeField]
    private float buffAbilityPower;

    [SerializeField]
    private float buffArmor;

    [SerializeField]
    private float buffResist;

    [SerializeField]
    private float buffAttackSpeed;

    [SerializeField]
    private float buffMoveSpeed;

    [SerializeField]
    private float buffCooldown;

    //public int Id { get => id; }

    public bool IsInvisible { get => isInvisible; }

    public float LifeSteal { get => lifeSteal; }

    public float BuffMaxHealth { get => buffMaxHealth; }

    public float BuffMaxMana { get => buffMaxMana; }

    public float BuffAttackDamage { get => buffAttackDamage; }

    public float BuffAbilityPower { get => buffAbilityPower; }

    public float BuffArmor { get => buffArmor; }

    public float BuffResist { get => buffResist; }

    public float BuffAttackSpeed { get => buffAttackSpeed; }

    public float BuffMoveSpeed { get => buffMoveSpeed; }

    public float BuffCooldown { get => buffCooldown; }

    public StatModifier CreateInstance()
    {
        return new StatModifier
        {
            //Id = Id,
            IsInvisible = IsInvisible,
            LifeSteal = LifeSteal,
            BuffMaxHealth = BuffMaxHealth,
            BuffMaxMana = BuffMaxMana,
            BuffAttackDamage = BuffAttackDamage,
            BuffAbilityPower = BuffAbilityPower,
            BuffArmor = BuffArmor,
            BuffResist = BuffResist,
            BuffAttackSpeed = BuffAttackSpeed,
            BuffMoveSpeed = BuffMoveSpeed,
            BuffCooldown = BuffCooldown
        };
    }
}

public struct StatModifier
{
    //public int Id { get; set; }

    public bool IsInvisible { get; set; }

    public float LifeSteal { get; set; }

    public float BuffMaxHealth { get; set; }

    public float BuffMaxMana { get; set; }

    public float BuffAttackDamage { get; set; }

    public float BuffAbilityPower { get; set; }

    public float BuffArmor { get; set; }

    public float BuffResist { get; set; }

    public float BuffAttackSpeed { get; set; }

    public float BuffMoveSpeed { get; set; }

    public float BuffCooldown { get; set; }

    public static StatModifier operator +(StatModifier a, StatModifier b)
    {
        return new StatModifier
        {
            //Id = a.Id,
            IsInvisible = a.IsInvisible || b.IsInvisible,
            LifeSteal = a.LifeSteal + b.LifeSteal,
            BuffMaxHealth = a.BuffMaxHealth + b.BuffMaxHealth,
            BuffMaxMana = a.BuffMaxMana + b.BuffMaxMana,
            BuffAttackDamage = a.BuffAttackDamage + b.BuffAttackDamage,
            BuffAbilityPower = a.BuffAbilityPower + b.BuffAbilityPower,
            BuffArmor = a.BuffArmor + b.BuffArmor,
            BuffResist = a.BuffResist + b.BuffResist,
            BuffAttackSpeed = a.BuffAttackSpeed + b.BuffAttackSpeed,
            BuffMoveSpeed = a.BuffMoveSpeed + b.BuffMoveSpeed,
            BuffCooldown = a.BuffCooldown + b.BuffCooldown
        };
    }
}