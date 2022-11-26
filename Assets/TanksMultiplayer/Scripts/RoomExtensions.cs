using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public static class RoomExtensions
{
    public const string score = "score";

    public static int[] GetScore(this Room room)
    {
        if (room.CustomProperties.TryGetValue(score, out object value))
        {
            return (int[])value;
        }

        return new int[] { 0, 0 };
    }

    public static int[] AddScore(this Room room, int teamIndex, int value)
    {
        int[] scores = room.GetScore();

        scores[teamIndex] += value;

        room.SetCustomProperties(new Hashtable() { { score, scores } });

        return scores;
    }
}

/*public enum NetworkMode
{
    Online = 0,
    LAN = 1,
    Offline = 2
}*/