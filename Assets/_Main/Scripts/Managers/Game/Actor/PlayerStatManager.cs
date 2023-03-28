using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatManager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region Serializable

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

    #endregion

    #region Components

    private PlayerInventoryManager inventory;

    private PlayerStatusManager status;

    #endregion

    #region Networked

    private int kills;

    private int deaths;
    
    private int health;

    private int mana;

    private bool hasKey;

    private bool hasChest;

    private bool hasChestTeam0;

    private bool hasChestTeam1;

    #endregion

    #region Accessors

    public int BaseMaxHealth { get => maxHealth; }

    public int BaseMaxMana { get => maxMana; }

    public int BaseAbilityPower { get => abilityPower; }

    public int BaseAttackDamage { get => attackDamage; }

    public int BaseAttackSpeed { get => attackSpeed; }

    public int BaseMoveSpeed { get => moveSpeed; }

    public int BaseArmor { get => armor; }

    public int BaseResist { get => resist; }
    
    public int Health { get => health; }

    public int Mana { get => mana; }

    public bool HasKey { get => hasKey; }

    public bool HasChest { get => hasChest; }

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

    #endregion

    public int MaxHealth(float modifier = 0) => (int)(maxHealth + Inventory.StatModifier.BuffMaxHealth + Status.StatModifier.BuffMaxHealth + modifier);

    public int MaxMana(float modifier = 0) => (int)(maxMana * (1 + Inventory.StatModifier.BuffMaxMana + Status.StatModifier.BuffMaxMana + modifier));

    public int AbilityPower(float modifier = 0) => (int)(abilityPower * (1 + Inventory.StatModifier.BuffAbilityPower + Status.StatModifier.BuffAbilityPower + modifier));

    public int AttackDamage(float modifier = 0) => (int)(attackDamage * (1 + Inventory.StatModifier.BuffAttackDamage + Status.StatModifier.BuffAttackDamage + modifier));

    public int AttackSpeed(float modifier = 0) => (int)(attackSpeed * (1 + Inventory.StatModifier.BuffAttackSpeed + Status.StatModifier.BuffAttackSpeed + modifier));

    public int MoveSpeed(float modifier = 0) => (int)(moveSpeed * (1 + Inventory.StatModifier.BuffMoveSpeed + Status.StatModifier.BuffMoveSpeed + modifier));

    public int Armor(float modifier = 0) => (int)(armor * (1 + Inventory.StatModifier.BuffArmor + Status.StatModifier.BuffArmor + modifier));

    public int Resist(float modifier = 0) => (int)(resist * (1 + Inventory.StatModifier.BuffResist + Status.StatModifier.BuffResist + modifier));

    /*public bool HasChest(int team)
    {
        if (team == 0) return hasChestTeam0;
        if (team == 1) return hasChestTeam1;

        return false;
    }*/

    public void AddHealth(int amount)
    {
        health = Mathf.Clamp(health + amount, 0, MaxHealth());
    }

    public void AddMana(int amount)
    {
        mana = Mathf.Clamp(mana + amount, 0, MaxMana());
    }

    public void SetHealth(int amount)
    {
        health = Mathf.Clamp(amount, 0, MaxHealth());
    }

    public void SetMana(int amount)
    {
        mana = Mathf.Clamp(amount, 0, MaxMana());
    }

    public void SetKey(bool value)
    {
        hasKey = value;
    }

    public void SetChest(bool value)
    {
        hasChest = value;
    }

    public void SetChest(bool value, int team)
    {
        if (team == 0) hasChestTeam0 = value;
        if (team == 1) hasChestTeam1 = value;
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
            stream.SendNext(hasKey);
            stream.SendNext(hasChest);
            //stream.SendNext(hasChestTeam0);
            //stream.SendNext(hasChestTeam1);
        }
        else
        {
            health = (int)stream.ReceiveNext();
            mana = (int)stream.ReceiveNext();
            kills = (int)stream.ReceiveNext();
            deaths = (int)stream.ReceiveNext();
            hasKey = (bool)stream.ReceiveNext();
            hasChest = (bool)stream.ReceiveNext();
            //hasChestTeam0 = (bool)stream.ReceiveNext();
            //hasChestTeam1 = (bool)stream.ReceiveNext();
        }
    }

    private IEnumerator YieldManaAutoRegen(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);

            AddMana(MaxMana() / 10);
        }
    }
}
