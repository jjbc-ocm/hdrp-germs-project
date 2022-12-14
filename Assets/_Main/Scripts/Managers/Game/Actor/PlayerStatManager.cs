using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    private int maxHealth = 175;

    [SerializeField]
    private int maxMana = 100;

    [SerializeField]
    private int attackDamage = 50;

    [SerializeField]
    private int abilityPower = 50;

    [SerializeField]
    private int armor = 50;

    [SerializeField]
    private int resist = 50;

    [SerializeField]
    private int attackSpeed = 50;

    [SerializeField]
    private int moveSpeed = 50;

    private int kills;

    private int deaths;

    private PlayerInventoryManager inventory;

    private PlayerStatusManager status;

    private int health;

    private int mana;

    public int BaseMaxHealth { get => maxHealth; }

    public int BaseMaxMana { get => maxMana; }

    public int BaseAbilityPower { get => abilityPower; }

    public int BaseAttackDamage { get => attackDamage; }

    public int BaseAttackSpeed { get => attackSpeed; }

    public int BaseMoveSpeed { get => moveSpeed; }

    public int BaseArmor { get => armor; }

    public int BaseResist { get => resist; }

    public int MaxHealth { get => (int)(maxHealth + Inventory.StatModifier.BuffMaxHealth + Status.StatModifier.BuffMaxHealth); }

    public int MaxMana { get => (int)(maxMana * (1 + Inventory.StatModifier.BuffMaxMana + Status.StatModifier.BuffMaxMana)); }

    public int AbilityPower { get => (int)(abilityPower * (1 + Inventory.StatModifier.BuffAbilityPower + Status.StatModifier.BuffAbilityPower)); }

    public int AttackDamage { get => (int)(attackDamage * (1 + Inventory.StatModifier.BuffAttackDamage + Status.StatModifier.BuffAttackDamage)); }

    public int AttackSpeed { get => (int)(attackSpeed * (1 + Inventory.StatModifier.BuffAttackSpeed + Status.StatModifier.BuffAttackSpeed)); }

    public int MoveSpeed { get => (int)(moveSpeed * (1 + Inventory.StatModifier.BuffMoveSpeed + Status.StatModifier.BuffMoveSpeed)); }

    public int Armor { get => (int)(armor * (1 + Inventory.StatModifier.BuffArmor + Status.StatModifier.BuffArmor)); }

    public int Resist { get => (int)(resist * (1 + Inventory.StatModifier.BuffResist + Status.StatModifier.BuffResist)); }

    public int Health { get => health; }

    public int Mana { get => mana; }

    public int Kills { get => kills; }

    public int Deaths { get => deaths; }

    public PlayerInventoryManager Inventory
    {
        get
        {
            if (inventory == null)
            {
                inventory = GetComponent<PlayerInventoryManager>();
            }

            return inventory;
        }
    }

    public PlayerStatusManager Status
    {
        get
        {
            if (status == null)
            {
                status = GetComponent<PlayerStatusManager>();
            }

            return status;
        }
    }


    public void AddHealth(int amount)
    {
        health = Mathf.Clamp(health + amount, 0, MaxHealth);
    }

    public void AddMana(int amount)
    {
        mana = Mathf.Clamp(mana + amount, 0, MaxMana);
    }

    public void SetHealth(int amount)
    {
        health = Mathf.Clamp(amount, 0, MaxHealth);
    }

    public void SetMana(int amount)
    {
        mana = Mathf.Clamp(amount, 0, MaxMana);
    }

    public void AddKill()
    {
        kills++;
    }

    public void AddDeath()
    {
        deaths++;
    }

    void Start()
    {
        if (!photonView.IsMine) return;

        StartCoroutine(YieldManaAutoRegen(1)); // TODO: it actually more efficient if it was placed in update method
    }

    public void Initialize()
    {
        health = maxHealth;

        mana = maxMana;
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
            stream.SendNext(mana);
            stream.SendNext(kills);
            stream.SendNext(deaths);
        }
        else
        {
            health = (int)stream.ReceiveNext();
            mana = (int)stream.ReceiveNext();
            kills = (int)stream.ReceiveNext();
            deaths = (int)stream.ReceiveNext();
        }
    }

    private IEnumerator YieldManaAutoRegen(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);

            AddMana(MaxMana / 10);
        }
    }
}
