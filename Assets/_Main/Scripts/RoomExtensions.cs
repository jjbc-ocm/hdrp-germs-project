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

    public const string chestLost = "chestLost";

    public const string gameMode = "gameMode";

    public const string bot0 = "bot0";
    public const string bot1 = "bot1";
    public const string bot2 = "bot2";
    public const string bot3 = "bot3";
    public const string bot4 = "bot4";
    public const string bot5 = "bot5";

    public static void Initialize(this Room room, bool isTeamSetup, GameMode gameMode)
    {
        room.SetCustomProperties(new Hashtable
        {
            { RoomExtensions.isTeamSetup, isTeamSetup },
            { RoomExtensions.gameMode, gameMode },
            { bot0, JsonUtility.ToJson(new BotInfo { BotIndex = 0, Name = "Bot 1", Team = 0, ShipIndex = 0 }) },
            { bot1, JsonUtility.ToJson(new BotInfo { BotIndex = 1, Name = "Bot 2", Team = 0, ShipIndex = 1 }) },
            { bot2, JsonUtility.ToJson(new BotInfo { BotIndex = 2, Name = "Bot 3", Team = 0, ShipIndex = 2 }) },
            { bot3, JsonUtility.ToJson(new BotInfo { BotIndex = 3, Name = "Bot 4", Team = 1, ShipIndex = 3 }) },
            { bot4, JsonUtility.ToJson(new BotInfo { BotIndex = 4, Name = "Bot 5", Team = 1, ShipIndex = 4 }) },
            { bot5, JsonUtility.ToJson(new BotInfo { BotIndex = 5, Name = "Bot 6", Team = 1, ShipIndex = 5 }) },
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

    public static int GetScore(this Room room, int index)
    {
        if (room.CustomProperties.TryGetValue(score + index, out object value))
        {
            return (int)value;
        }

        return 0;
    }

    public static int GetChestLost(this Room room, int team)
    {
        if (room.CustomProperties.TryGetValue(chestLost + team, out object value))
        {
            return (int)value;
        }

        return 0;
    }

    public static GameMode GetGameMode(this Room room)
    {
        if (room.CustomProperties.TryGetValue(gameMode, out object value))
        {
            return (GameMode)value;
        }
        return GameMode.Standard;
    }

    public static void AddScoreByKill(this Room room, int teamGain, int value)
    {
        var data = new Hashtable
        {
            { score + teamGain, room.GetScore(teamGain) + value }
        };

        room.SetCustomProperties(data);
    }

    public static void AddScoreByChest(this Room room, int teamGain, int teamLose, int value)
    {
        var data = new Hashtable
        {
            { score + teamGain, room.GetScore(teamGain) + value },
            { chestLost + teamLose, room.GetChestLost(teamLose) + 1 }
        };

        room.SetCustomProperties(data);
    }

    public static void AddGameMode(this Room room, GameMode gameMode)
    {
        var data = new Hashtable
        {
            { gameMode, gameMode }
        };

        room.SetCustomProperties(data);
    }

    public static List<BotInfo> GetBots(this Room room)
    {
        List<BotInfo> bots = null;

        for (var i = 0; i < 6; i++)
        {
            if (room.CustomProperties.TryGetValue("bot" + i, out object value))
            {
                if (bots == null) bots = new List<BotInfo>();

                bots.Add(JsonUtility.FromJson<BotInfo>((string)value));
            }
        }
        
        return bots;
    }

    public static BotInfo GetBot(this Room room, int index)
    {
        if (room.CustomProperties.TryGetValue("bot" + index, out object value))
        {
            return JsonUtility.FromJson<BotInfo>((string)value);
        }

        return null;
    }

    public static void SetBotInfo(this Room room, int index, BotInfo botInfo)
    {
        room.SetCustomProperties(new Hashtable
        {
            { "bot" + index, JsonUtility.ToJson(botInfo) }
        });
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