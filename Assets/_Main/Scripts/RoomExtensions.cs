using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public static class RoomExtensions
{
    public const string score = "score";

    public const string chest = "chest";

    public static int[] GetScore(this Room room)
    {
        if (room.CustomProperties.TryGetValue(score, out object value))
        {
            return (int[])value;
        }

        return new int[] { 0, 0 };
    }

    public static int[] GetChests(this Room room)
    {
        if (room.CustomProperties.TryGetValue(chest, out object value))
        {
            return (int[])value;
        }

        return new int[] { 0, 0 };
    }

    public static void AddScore(this Room room, int teamIndex, int value, bool isFromChest)
    {
        int[] scores = room.GetScore();

        int[] chests = room.GetChests();

        scores[teamIndex] += value;

        var data = new Hashtable
        {
            { score, scores }
        };

        if (isFromChest)
        {
            chests[teamIndex] += 1;

            data.Add(chest, chests);
        }

        //room.SetCustomProperties(new Hashtable() { { score, scores } });
        room.SetCustomProperties(data);
    }
}