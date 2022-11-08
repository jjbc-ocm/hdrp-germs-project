/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace TanksMP
{
    /// <summary>
    /// This class extends Photon's PhotonPlayer object by custom properties.
    /// Provides several methods for setting and getting variables out of them.
    /// </summary>
    public static class PlayerExtensions
    {
        public const string team = "team";

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
        public const string gold = "gold";

        #region For Photon View

        public static string GetName(this PhotonView player)
        {
            return player.Owner.NickName;
        }

        public static int GetTeam(this PhotonView player)
        {
            return player.Owner.GetTeam();
        }

        public static bool HasChest(this PhotonView player)
        {
            return player.Owner.HasChest();
        }

        public static int GetHealth(this PhotonView player)
        {
            return player.Owner.GetHealth();
        }

        public static int GetMana(this PhotonView player)
        {
            return player.Owner.GetMana();
        }

        public static int GetRegen(this PhotonView player)
        {
            return player.Owner.GetRegen();
        }

        public static int GetAttackDamage(this PhotonView player)
        {
            return player.Owner.GetAttackDamage();
        }

        public static int GetAbilityPower(this PhotonView player)
        {
            return player.Owner.GetAbilityPower();
        }

        public static int GetArmor(this PhotonView player)
        {
            return player.Owner.GetArmor();
        }

        public static int GetResist(this PhotonView player)
        {
            return player.Owner.GetResist();
        }

        public static int GetAttackSpeed(this PhotonView player)
        {
            return player.Owner.GetAttackSpeed();
        }

        public static int GetMoveSpeed(this PhotonView player)
        {
            return player.Owner.GetMoveSpeed();
        }

        public static int GetGold(this PhotonView player)
        {
            return player.Owner.GetGold();
        }

        public static void HasChest(this PhotonView player, bool value)
        {
            player.Owner.HasChest(value);
        }

        public static void SetHealth(this PhotonView player, int value)
        {
            player.Owner.SetHealth(value);
        }

        public static void SetMana(this PhotonView player, int value)
        {
            player.Owner.SetMana(value);
        }

        public static void SetRegen(this PhotonView player, int value)
        {
            player.Owner.SetRegen(value);
        }

        public static void SetAttackDamage(this PhotonView player, int value)
        {
            player.Owner.SetAttackDamage(value);
        }

        public static void SetAbilityPower(this PhotonView player, int value)
        {
            player.Owner.SetAbilityPower(value);
        }

        public static void SetArmor(this PhotonView player, int value)
        {
            player.Owner.SetArmor(value);
        }

        public static void SetResist(this PhotonView player, int value)
        {
            player.Owner.SetResist(value);
        }

        public static void SetAttackSpeed(this PhotonView player, int value)
        {
            player.Owner.SetAttackSpeed(value);
        }

        public static void SetMoveSpeed(this PhotonView player, int value)
        {
            player.Owner.SetMoveSpeed(value);
        }

        public static int AddGold(this PhotonView player, int value)
        {
            return player.Owner.AddGold(value);
        }

        public static int RemoveGold(this PhotonView player, int value)
        {
            return player.Owner.RemoveGold(value);
        }

        public static void Clear(this PhotonView player)
        {
            player.Owner.Clear();
        }

        #endregion

        #region For Photon Player

        public static bool HasChest(this Photon.Realtime.Player player)
        {
            return System.Convert.ToBoolean(player.CustomProperties[hasChest]);
        }

        public static int GetHealth(this Photon.Realtime.Player player)
        {
            return System.Convert.ToInt32(player.CustomProperties[health]);
        }

        public static int GetMana(this Photon.Realtime.Player player)
        {
            return System.Convert.ToInt32(player.CustomProperties[mana]);
        }

        public static int GetRegen(this Photon.Realtime.Player player)
        {
            return System.Convert.ToInt32(player.CustomProperties[regen]);
        }

        public static int GetAttackDamage(this Photon.Realtime.Player player)
        {
            return System.Convert.ToInt32(player.CustomProperties[attackDamage]);
        }

        public static int GetAbilityPower(this Photon.Realtime.Player player)
        {
            return System.Convert.ToInt32(player.CustomProperties[abilityPower]);
        }

        public static int GetArmor(this Photon.Realtime.Player player)
        {
            return System.Convert.ToInt32(player.CustomProperties[armor]);
        }

        public static int GetResist(this Photon.Realtime.Player player)
        {
            return System.Convert.ToInt32(player.CustomProperties[resist]);
        }

        public static int GetAttackSpeed(this Photon.Realtime.Player player)
        {
            return System.Convert.ToInt32(player.CustomProperties[attackSpeed]);
        }

        public static int GetMoveSpeed(this Photon.Realtime.Player player)
        {
            return System.Convert.ToInt32(player.CustomProperties[moveSpeed]);
        }

        public static int GetGold(this Photon.Realtime.Player player)
        {
            return System.Convert.ToInt32(player.CustomProperties[gold]);
        }

        public static void SetTeam(this Photon.Realtime.Player player, int teamIndex)
        {
            player.SetCustomProperties(new Hashtable() { { team, (byte)teamIndex } });
        }

        public static void HasChest(this Photon.Realtime.Player player, bool value)
        {
            player.SetCustomProperties(new Hashtable() { { hasChest, value } });
        }

        public static void SetHealth(this Photon.Realtime.Player player, int value)
        {
            player.SetCustomProperties(new Hashtable() { { health, (byte)value } });
        }

        public static void SetMana(this Photon.Realtime.Player player, int value)
        {
            player.SetCustomProperties(new Hashtable() { { mana, (byte)value } });
        }

        public static void SetRegen(this Photon.Realtime.Player player, int value)
        {
            player.SetCustomProperties(new Hashtable() { { regen, (byte)value } });
        }

        public static void SetAttackDamage(this Photon.Realtime.Player player, int value)
        {
            player.SetCustomProperties(new Hashtable() { { attackDamage, (byte)value } });
        }

        public static void SetAbilityPower(this Photon.Realtime.Player player, int value)
        {
            player.SetCustomProperties(new Hashtable() { { abilityPower, (byte)value } });
        }

        public static void SetArmor(this Photon.Realtime.Player player, int value)
        {
            player.SetCustomProperties(new Hashtable() { { armor, (byte)value } });
        }

        public static void SetResist(this Photon.Realtime.Player player, int value)
        {
            player.SetCustomProperties(new Hashtable() { { resist, (byte)value } });
        }

        public static void SetAttackSpeed(this Photon.Realtime.Player player, int value)
        {
            player.SetCustomProperties(new Hashtable() { { attackSpeed, (byte)value } });
        }

        public static void SetMoveSpeed(this Photon.Realtime.Player player, int value)
        {
            player.SetCustomProperties(new Hashtable() { { moveSpeed, (byte)value } });
        }

        public static int AddGold(this Photon.Realtime.Player player, int value)
        {
            int goldValue = player.GetGold();
            goldValue += value;
            player.SetCustomProperties(new Hashtable() { { gold, goldValue } });
            return goldValue;
        }

        public static int RemoveGold(this Photon.Realtime.Player player, int value)
        {
            int goldValue = player.GetGold();
            goldValue -= value;
            if (goldValue < 0)
            {
                goldValue = 0;
            }
            player.SetCustomProperties(new Hashtable() { { gold, goldValue } });
            return goldValue;
        }

        public static void Clear(this Photon.Realtime.Player player)
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
                    { gold, (byte)0 }
                });

        }

        #endregion


    }
}
