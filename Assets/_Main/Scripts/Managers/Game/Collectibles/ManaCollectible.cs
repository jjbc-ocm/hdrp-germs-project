/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;

namespace TanksMP
{
	public class ManaCollectible : Collectible
    {
        [SerializeField]
        private int amount = 100;

        protected override void OnObtain(PlayerManager player)
        {
            Debug.Log("Collectible Mana");

            player.Stat.AddMana(amount);

            Debug.Log("Collectible Mana End");
        }

        /*public override bool Apply(Player p)
        {
            if (p == null)
                return false;

            int value = p.photonView.GetMana() + amount;

            value = Mathf.Min(value, p.MaxMana);

            p.photonView.SetMana(value);

            return true;
        }*/
    }
}
