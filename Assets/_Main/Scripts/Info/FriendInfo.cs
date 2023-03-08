using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendInfo
{
    private CSteamID steamId;

    private string name;

    private EPersonaState status;

    public CSteamID SteamID { get => steamId; set => steamId = value; }

    public string Name { get => name; set => name = value; }

    public EPersonaState Status { get => status; set => status = value; }
}
