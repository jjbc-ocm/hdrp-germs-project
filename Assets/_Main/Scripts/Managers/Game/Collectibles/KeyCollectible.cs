using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCollectible : Collectible
{
    protected override void OnObtain(PlayerManager player)
    {
        player.Stat.SetKey(true);
    }
}