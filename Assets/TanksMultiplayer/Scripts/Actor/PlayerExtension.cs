using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public static class PlayerExtension
{
    public const string team = "team";
    public const string shipIndex = "shipIndex";

    public const string hasChest = "hasChest";
    public const string health = "health";
    public const string mana = "mana";
    public const string regen = "regen";
    public const string attackDamage = "attackDamage";
    public const string abilityPower = "abilityPower";
    public const string armor = "armor";
    public const string resist = "resist";
    public const string attackSpeed = "attackSpeed";
    public const string moveSpeed = "moveSpeed";
    //public const string gold = "gold";
    //public const string itemPrefix = "item_";

    public static int m_selectedShipIdx = 0;

    public static List<GPDummyData> m_dummySlots = new List<GPDummyData>();
    public static int m_selectedDummySlot = 0;

    #region For Photon View

    public static string GetName(this PhotonView view)
    {
        return view.Owner.NickName;
    }

    public static int GetTeam(this PhotonView view)
    {
        return view.Owner.GetTeam();
    }

    public static bool HasChest(this PhotonView view)
    {
        return view.Owner.HasChest();
    }

    public static int GetHealth(this PhotonView view)
    {
        return view.Owner.GetHealth();
    }

    public static int GetMana(this PhotonView view)
    {
        return view.Owner.GetMana();
    }

    public static int GetRegen(this PhotonView view)
    {
        return view.Owner.GetRegen();
    }

    public static int GetAttackDamage(this PhotonView view)
    {
        return view.Owner.GetAttackDamage();
    }

    public static int GetAbilityPower(this PhotonView view)
    {
        return view.Owner.GetAbilityPower();
    }

    public static int GetArmor(this PhotonView view)
    {
        return view.Owner.GetArmor();
    }

    public static int GetResist(this PhotonView view)
    {
        return view.Owner.GetResist();
    }

    public static int GetAttackSpeed(this PhotonView view)
    {
        return view.Owner.GetAttackSpeed();
    }

    public static int GetMoveSpeed(this PhotonView view)
    {
        return view.Owner.GetMoveSpeed();
    }

    /*public static int GetGold(this PhotonView view)
    {
        return view.Owner.GetGold();
    }*/

    /*public static List<ItemCountData> GetItems(this PhotonView view, ItemData[] data)
    {
        return view.Owner.GetItems(data);
    }*/

    /*public static ItemCountData GetItem(this PhotonView view, ItemData data)
    {
        return view.Owner.GetItem(data);
    }*/

    public static void HasChest(this PhotonView view, bool value)
    {
        view.Owner.HasChest(value);
    }

    public static void SetHealth(this PhotonView view, int value)
    {
        view.Owner.SetHealth(value);
    }

    public static void SetMana(this PhotonView view, int value)
    {
        view.Owner.SetMana(value);
    }

    public static void SetRegen(this PhotonView view, int value)
    {
        view.Owner.SetRegen(value);
    }

    public static void SetAttackDamage(this PhotonView view, int value)
    {
        view.Owner.SetAttackDamage(value);
    }

    public static void SetAbilityPower(this PhotonView view, int value)
    {
        view.Owner.SetAbilityPower(value);
    }

    public static void SetArmor(this PhotonView view, int value)
    {
        view.Owner.SetArmor(value);
    }

    public static void SetResist(this PhotonView view, int value)
    {
        view.Owner.SetResist(value);
    }

    public static void SetAttackSpeed(this PhotonView view, int value)
    {
        view.Owner.SetAttackSpeed(value);
    }

    public static void SetMoveSpeed(this PhotonView view, int value)
    {
        view.Owner.SetMoveSpeed(value);
    }

    /*public static int AddGold(this PhotonView view, int value)
    {
        return view.Owner.AddGold(value);
    }

    public static int RemoveGold(this PhotonView view, int value)
    {
        return view.Owner.RemoveGold(value);
    }*/

    /*public static void PurchaseItem(this PhotonView view, ItemData item, int quantity)
    {
        view.Owner.PurchaseItem(item, quantity);
    }*/

    /*public static void ConsumeItem(this PhotonView view, ItemData item)
    {
        view.Owner.ConsumeItem(item);
    }*/

    public static void Clear(this PhotonView view)
    {
        view.Owner.Clear();
    }

    #endregion

    #region For Photon Player

    public static void Initialize(this Player player, int team, int shipIndex)
    {
        player.SetCustomProperties(new Hashtable
        {
            { PlayerExtension.team, team },
            { PlayerExtension.shipIndex, shipIndex }
        });
    }

    public static void SetSelectedShipIdx(this Player player, int shipIndex)
    {
        m_selectedShipIdx = shipIndex; // not saved on custom properties yet because he set it outside of room.
    }

    public static int GetTeam(this Player player)
    {
        if (player.CustomProperties.TryGetValue(team, out object value))
        {
            return System.Convert.ToInt32(value);
        }

        return -1;
    }

    public static int GetShipIndex(this Player player)
    {
        if (player.CustomProperties.TryGetValue(shipIndex, out object value))
        {
            return System.Convert.ToInt32(value);
        }

        return 0;
    }

    public static int GetSelectedShipIdx(this Player player)
    {
        return m_selectedShipIdx;
    }

    public static bool HasChest(this Player player)
    {
        return System.Convert.ToBoolean(player.CustomProperties[hasChest]);
    }

    public static int GetHealth(this Player player)
    {
        return System.Convert.ToInt32(player.CustomProperties[health]);
    }

    public static int GetMana(this Player player)
    {
        return System.Convert.ToInt32(player.CustomProperties[mana]);
    }

    public static int GetRegen(this Player player)
    {
        return System.Convert.ToInt32(player.CustomProperties[regen]);
    }

    public static int GetAttackDamage(this Player player)
    {
        return System.Convert.ToInt32(player.CustomProperties[attackDamage]);
    }

    public static int GetAbilityPower(this Player player)
    {
        return System.Convert.ToInt32(player.CustomProperties[abilityPower]);
    }

    public static int GetArmor(this Player player)
    {
        return System.Convert.ToInt32(player.CustomProperties[armor]);
    }

    public static int GetResist(this Player player)
    {
        return System.Convert.ToInt32(player.CustomProperties[resist]);
    }

    public static int GetAttackSpeed(this Player player)
    {
        return System.Convert.ToInt32(player.CustomProperties[attackSpeed]);
    }

    public static int GetMoveSpeed(this Player player)
    {
        return System.Convert.ToInt32(player.CustomProperties[moveSpeed]);
    }

    /*public static int GetGold(this Player player)
    {
        return System.Convert.ToInt32(player.CustomProperties[gold]);
    }*/

    /*public static List<ItemCountData> GetItems(this Player player, ItemData[] data)
    {
        var itemCounts = new List<ItemCountData>();

        foreach (var datum in data)
        {
            if (player.CustomProperties.TryGetValue(itemPrefix + datum.Name, out object value))
            {
                itemCounts.Add(new ItemCountData
                {
                    Item = datum,

                    Count = System.Convert.ToInt32(value)
                });
            }
        }

        return itemCounts;
    }*/

    /*public static ItemCountData GetItem(this Player player, ItemData data)
    {
        if (player.CustomProperties.TryGetValue(itemPrefix + data.Name, out object value))
        {
            return new ItemCountData
            {
                Item = data,

                Count = System.Convert.ToInt32(value)
            };
        }

        return new ItemCountData
        {
            Item = data,

            Count = 0
        };
    }*/

    public static void SetTeam(this Player player, int teamIndex)
    {
        player.SetCustomProperties(new Hashtable() { { team, (byte)teamIndex } });
    }

    public static void HasChest(this Player player, bool value)
    {
        player.SetCustomProperties(new Hashtable() { { hasChest, value } });
    }

    public static void SetHealth(this Player player, int value)
    {
        player.SetCustomProperties(new Hashtable() { { health, (byte)value } });
    }

    public static void SetMana(this Player player, int value)
    {
        player.SetCustomProperties(new Hashtable() { { mana, (byte)value } });
    }

    public static void SetRegen(this Player player, int value)
    {
        player.SetCustomProperties(new Hashtable() { { regen, (byte)value } });
    }

    public static void SetAttackDamage(this Player player, int value)
    {
        player.SetCustomProperties(new Hashtable() { { attackDamage, (byte)value } });
    }

    public static void SetAbilityPower(this Player player, int value)
    {
        player.SetCustomProperties(new Hashtable() { { abilityPower, (byte)value } });
    }

    public static void SetArmor(this Player player, int value)
    {
        player.SetCustomProperties(new Hashtable() { { armor, (byte)value } });
    }

    public static void SetResist(this Player player, int value)
    {
        player.SetCustomProperties(new Hashtable() { { resist, (byte)value } });
    }

    public static void SetAttackSpeed(this Player player, int value)
    {
        player.SetCustomProperties(new Hashtable() { { attackSpeed, (byte)value } });
    }

    public static void SetMoveSpeed(this Player player, int value)
    {
        player.SetCustomProperties(new Hashtable() { { moveSpeed, (byte)value } });
    }

    /*public static int AddGold(this Player player, int value)
    {
        int goldValue = player.GetGold();
        goldValue += value;
        player.SetCustomProperties(new Hashtable() { { gold, goldValue } });
        return goldValue;
    }*/

    /*public static int RemoveGold(this Player player, int value)
    {
        int goldValue = player.GetGold();
        goldValue -= value;
        if (goldValue < 0)
        {
            goldValue = 0;
        }
        player.SetCustomProperties(new Hashtable() { { gold, goldValue } });
        return goldValue;
    }*/

    /*public static void PurchaseItem(this Player player, ItemData item, int quantity)
    {
        var goldValue = player.GetGold();

        var itemCount = player.GetItem(item);

        *//* Buy *//*
        if (quantity > 0)
        {
            player.SetCustomProperties(new Hashtable
                {
                    { gold, goldValue - item.CostBuy },
                    { itemPrefix + item.Name, itemCount.Count + 1 }
                });
        }

        *//* Sell *//*
        if (quantity < 0)
        {
            player.SetCustomProperties(new Hashtable
                {
                    { gold, goldValue + item.CostSell },
                    { itemPrefix + item.Name, itemCount.Count - 1 }
                });
        }
    }*/

   /* public static void ConsumeItem(this Player player, ItemData item)
    {
        var itemCount = player.GetItem(item);

        player.SetCustomProperties(new Hashtable
        {
            { itemPrefix + item.Name, itemCount.Count - 1 }
        });
    }*/

    public static void SetSelectedDummySlot(this Player player, int slotIndex)
    {
        m_selectedDummySlot = slotIndex; // not saved on custom properties yet because he set it outside of room.
    }

    public static void Clear(this Player player)
    {
        player.SetCustomProperties(
            new Hashtable()
            {
                    { health, (byte)0 },
                    { regen, (byte)0 },
                    { attackDamage, (byte)0 },
                    { abilityPower, (byte)0 },
                    { armor, (byte)0 },
                    { resist, (byte)0 },
                    { attackSpeed, (byte)0 },
                    { moveSpeed, (byte)0 },
                    //{ gold, (byte)0 }
            });

    }

    #endregion
}
