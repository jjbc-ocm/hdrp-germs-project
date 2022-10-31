using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    public const string NETWORK_VERSION = "1";

    public const string MENU_SCENE_NAME = "Intro";

    public const string GAME_SCENE_NAME = "CTF_Game";

    public const string KEY_TEAM = "team";

    public const string KEY_SHIP_INDEX = "shipIndex";

    public const string KEY_PLAYER_INDEX = "playerIndex"; // TODO: Used where?

    public const int MAX_PLAYER_COUNT = 6;

    public const int MAX_TEAM = 2;

    public const int MAX_PLAYER_COUNT_PER_TEAM = MAX_PLAYER_COUNT / MAX_TEAM;

    public const int FOG_OF_WAR_DISTANCE = 150;
}
