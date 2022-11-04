using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerExtension
{
    public static void Initialize(this Player player, int team, int shipIndex)
    {
        player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            { Constants.KEY_TEAM, team },
            { Constants.KEY_SHIP_INDEX, shipIndex }
        });
    }

    public static int GetTeam(this Player player)
    {
        if (player.CustomProperties.TryGetValue(Constants.KEY_TEAM, out object value))
        {
            return System.Convert.ToInt32(value);
        }

        return 0;
    }

    public static int GetShipIndex(this Player player)
    {
        if (player.CustomProperties.TryGetValue(Constants.KEY_SHIP_INDEX, out object value))
        {
            return System.Convert.ToInt32(value);
        }

        return 0;
    }
}
