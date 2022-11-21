using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatusGroupData
{
    [SerializeField]
    private int id;

    [SerializeField]
    private bool isInvisible;

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
    private float duration;

    public int Id { get => id; }

    public bool IsInvisible { get => isInvisible; }

    public float BuffMaxHealth { get => buffMaxHealth; }

    public float BuffMaxMana { get => buffMaxMana; }

    public float BuffAttackDamage { get => buffAttackDamage; }

    public float BuffAbilityPower { get => buffAbilityPower; }

    public float BuffArmor { get => buffArmor; }

    public float BuffResist { get => buffResist; }

    public float BuffAttackSpeed { get => buffAttackSpeed; }

    public float BuffMoveSpeed { get => buffMoveSpeed; }

    public float Duration { get => duration; }

    public StatusGroup CreateInstance()
    {
        return new StatusGroup
        {
            Id = Id,
            IsInvisible = IsInvisible,
            BuffMaxHealth = BuffMaxHealth,
            BuffMaxMana = BuffMaxMana,
            BuffAttackDamage = BuffAttackDamage,
            BuffAbilityPower = BuffAbilityPower,
            BuffArmor = BuffArmor,
            BuffResist = BuffResist,
            BuffAttackSpeed = BuffAttackSpeed,
            BuffMoveSpeed = BuffMoveSpeed,
            Duration = Duration
        };
    }
}

public class StatusGroup
{
    private int id;

    private bool isInvisible;

    private float buffMaxHealth;

    private float buffMaxMana;

    private float buffAttackDamage;

    private float buffAbilityPower;

    private float buffArmor;

    private float buffResist;

    private float buffAttackSpeed;

    private float buffMoveSpeed;

    private float duration;

    public int Id { get => id; set => id = value; }

    public bool IsInvisible { get => isInvisible; set => isInvisible = value; }

    public float BuffMaxHealth { get => buffMaxHealth; set => buffMaxHealth = value; }

    public float BuffMaxMana { get => buffMaxMana; set => buffMaxMana = value; }

    public float BuffAttackDamage { get => buffAttackDamage; set => buffAttackDamage = value; }

    public float BuffAbilityPower { get => buffAbilityPower; set => buffAbilityPower = value; }

    public float BuffArmor { get => buffArmor; set => buffArmor = value; }

    public float BuffResist { get => buffResist; set => buffResist = value; }

    public float BuffAttackSpeed { get => buffAttackSpeed; set => buffAttackSpeed = value; }

    public float BuffMoveSpeed { get => buffMoveSpeed; set => buffMoveSpeed = value; }

    public float Duration { get => duration; set => duration = value; }
}