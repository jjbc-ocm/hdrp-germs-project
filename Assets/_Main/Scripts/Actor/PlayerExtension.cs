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
        var profile = GPPlayerProfile.m_instance;

        var playerData = APIManager.Instance.PlayerData;

        var dummyData = playerData.Dummy(playerData.SelectedDummyIndex).ToGPDummyData(
            profile.m_dummySkins,
            profile.m_dummyEyes,
            profile.m_dummyMouths,
            profile.m_dummyHairs,
            profile.m_dummyHorns,
            profile.m_dummyWears,
            profile.m_dummyGloves,
            profile.m_dummyTails);

        player.SetCustomProperties(new Hashtable
        {
            { PlayerExtension.team, team },
            { PlayerExtension.shipIndex, shipIndex },
            {"skin", dummyData.m_skin?.name },
            {"eyes", dummyData.m_eye?.name },
            {"mouth", dummyData.m_mouth?.name },
            {"hair", dummyData.m_hair?.name },
            {"horns", dummyData.m_horns?.name},
            {"wear", dummyData.m_wear?.name },
            {"gloves", dummyData.m_gloves?.name },
            {"tail", dummyData.m_tail?.name }
        });
    }

    /*public static void SetSelectedShipIdx(this Player player, int shipIndex)
    {
        m_selectedShipIdx = shipIndex; // not saved on custom properties yet because he set it outside of room.
    }*/

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

    /*public static void WriteDummyKeys(this Player player, GPDummyData data)
    {
        player.SetCustomProperties(new Hashtable
        {
            {"skin", data.m_skin?.name },
            {"eyes", data.m_eye?.name },
            {"mouth", data.m_mouth?.name },
            {"hair", data.m_hair?.name },
            {"horns", data.m_horns?.name},
            {"wear", data.m_wear?.name },
            {"gloves", data.m_gloves?.name },
            {"tail", data.m_tail?.name }
        });
    }*/

    public static List<string> GetDummyKeys(this Player player)
    {
        List<string> dummyKeys = new List<string>();
        if (player.CustomProperties.TryGetValue("skin", out object skin))
        {
            dummyKeys.Add((string)skin);
        }
        if (player.CustomProperties.TryGetValue("eyes", out object eyes))
        {
            dummyKeys.Add((string)eyes);
        }
        if (player.CustomProperties.TryGetValue("mouth", out object mouth))
        {
            dummyKeys.Add((string)mouth);
        }
        if (player.CustomProperties.TryGetValue("hair", out object hair))
        {
            dummyKeys.Add((string)hair);
        }
        if (player.CustomProperties.TryGetValue("horns", out object horns))
        {
            dummyKeys.Add((string)horns);
        }
        if (player.CustomProperties.TryGetValue("wear", out object wear))
        {
            dummyKeys.Add((string)wear);
        }
        if (player.CustomProperties.TryGetValue("gloves", out object gloves))
        {
            dummyKeys.Add((string)gloves);
        }
        if (player.CustomProperties.TryGetValue("tail", out object tail))
        {
            dummyKeys.Add((string)tail);
        }
        return dummyKeys;
    }

    #endregion
}
