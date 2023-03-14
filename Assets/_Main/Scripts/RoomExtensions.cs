using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public static class RoomExtensions
{
    public const string isTeamSetup = "isTeamSetup";

    public const string timePrepSceneLoaded = "timePrepSceneLoaded";

    public const string score = "score";

    public const string chest = "chest";

    public const string bots = "bots";
    
    public static void Initialize(this Room room, bool isTeamSetup)
    {
        room.SetCustomProperties(new Hashtable
        {
            { RoomExtensions.isTeamSetup, isTeamSetup },
            {
                bots,
                new string[]
                {
                    JsonUtility.ToJson(new BotInfo { Name = "Bot 1", Team = 0, ShipIndex = 0 }),
                    JsonUtility.ToJson(new BotInfo { Name = "Bot 2", Team = 0, ShipIndex = 1 }),
                    JsonUtility.ToJson(new BotInfo { Name = "Bot 3", Team = 0, ShipIndex = 2 }),
                    JsonUtility.ToJson(new BotInfo { Name = "Bot 4", Team = 1, ShipIndex = 3 }),
                    JsonUtility.ToJson(new BotInfo { Name = "Bot 5", Team = 1, ShipIndex = 4 }),
                    JsonUtility.ToJson(new BotInfo { Name = "Bot 6", Team = 1, ShipIndex = 5 })
                }
            }
        });
    }
    
    public static bool IsTeamSetup(this Room room)
    {
        if (room.CustomProperties.TryGetValue(isTeamSetup, out object value))
        {
            return (bool)value;
        }

        return false;
    }

    /*public static void IsTeamSetup(this Room room, bool value)
    {
        room.SetCustomProperties(new Hashtable
        {
            { isTeamSetup, value }
        });
    }*/

    public static double GetTimePrepSceneLoaded(this Room room)
    {
        if (room.CustomProperties.TryGetValue(timePrepSceneLoaded, out object value))
        {
            return (double)value;
        }

        return 0;
    }

    public static void SetTimePrepSceneLoaded(this Room room, double value)
    {
        room.SetCustomProperties(new Hashtable
        {
            { timePrepSceneLoaded, value }
        });
    }

    public static int[] GetScore(this Room room)
    {
        if (room.CustomProperties.TryGetValue(score, out object value))
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

        room.SetCustomProperties(data);
    }

    public static int[] GetChests(this Room room)
    {
        if (room.CustomProperties.TryGetValue(chest, out object value))
        {
            return (int[])value;
        }

        return new int[] { 0, 0 };
    }

    public static BotInfo[] GetBots(this Room room)
    {
        if (room.CustomProperties.TryGetValue(bots, out object value))
        {
            return ((string[])value).Select(i => JsonUtility.FromJson<BotInfo>(i)).ToArray();
        }

        return null;
    }

    

    

    /*public static void SetBots(this Room room)
    {
        room.SetCustomProperties(new Hashtable
        {
            { 
                bots, 
                new string[]
                {
                    JsonUtility.ToJson(new BotInfo { Name = "Bot 1", Team = 0, ShipIndex = 0 }),
                    JsonUtility.ToJson(new BotInfo { Name = "Bot 2", Team = 0, ShipIndex = 1 }),
                    JsonUtility.ToJson(new BotInfo { Name = "Bot 3", Team = 0, ShipIndex = 2 }),
                    JsonUtility.ToJson(new BotInfo { Name = "Bot 4", Team = 1, ShipIndex = 3 }),
                    JsonUtility.ToJson(new BotInfo { Name = "Bot 5", Team = 1, ShipIndex = 4 }),
                    JsonUtility.ToJson(new BotInfo { Name = "Bot 6", Team = 1, ShipIndex = 5 })
                }
            }
        });
    }*/
}