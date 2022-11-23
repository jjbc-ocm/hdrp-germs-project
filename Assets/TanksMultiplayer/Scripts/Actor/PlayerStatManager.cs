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

    private int health;

    private int mana;

    public int MaxHealth { get => maxHealth; }

    public int MaxMana { get => maxMana; }

    public int AbilityPower { get => abilityPower; }

    public int AttackDamage { get => attackDamage; }

    public int AttackSpeed { get => attackSpeed; }

    public int MoveSpeed { get => moveSpeed; }

    public int Armor { get => armor; }

    public int Resist { get => resist; }

    public int Health { get => health; }

    public int Mana { get => mana; }

    

    public void AddHealth(int amount)
    {
        health = Mathf.Clamp(health + amount, 0, maxHealth);
    }

    public void AddMana(int amount)
    {
        mana = Mathf.Clamp(mana + amount, 0, maxMana);
    }
    void Start()
    {
        if (!photonView.IsMine) return;

        health = maxHealth;

        mana = maxMana;

        StartCoroutine(YieldManaAutoRegen(1));
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
            stream.SendNext(mana);
            stream.SendNext(attackDamage);
            stream.SendNext(abilityPower);
            stream.SendNext(armor);
            stream.SendNext(resist);
            stream.SendNext(attackSpeed);
            stream.SendNext(moveSpeed);
        }
        else
        {
            health = (int)stream.ReceiveNext();
            mana = (int)stream.ReceiveNext();
            attackDamage = (int)stream.ReceiveNext();
            abilityPower = (int)stream.ReceiveNext();
            armor = (int)stream.ReceiveNext();
            resist = (int)stream.ReceiveNext();
            attackSpeed = (int)stream.ReceiveNext();
            moveSpeed = (int)stream.ReceiveNext();
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
