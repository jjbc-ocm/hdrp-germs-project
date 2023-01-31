﻿/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

namespace TanksMP
{
    public class CollectibleTeam : Collectible
    {
        protected override void OnObtain(Player player)
        {
            player.HasChest(true);

            GuideManager.Instance.TryAddChestGuide();
        }
    }
}