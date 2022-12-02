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

    public static void HasChest(this PhotonView view, bool value)
    {
        view.Owner.HasChest(value);
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

    public static void SetTeam(this Player player, int teamIndex)
    {
        player.SetCustomProperties(new Hashtable() { { team, (byte)teamIndex } });
    }

    public static void HasChest(this Player player, bool value)
    {
        player.SetCustomProperties(new Hashtable() { { hasChest, value } });
    }

    public static void SetSelectedDummySlot(this Player player, int slotIndex)
    {
        m_selectedDummySlot = slotIndex; // not saved on custom properties yet because he set it outside of room.
    }

    #endregion
}
