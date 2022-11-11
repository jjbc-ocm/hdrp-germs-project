/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;

namespace TanksMP
{
	public class HealthCollectible : Collectible
    {
        [SerializeField]
        private int amount = 5;

        protected override void OnObtain(Player player)
        {
            player.AddHealth(amount);
        }

        /*public override bool Apply(Player p)
        {
            if (p == null)
                return false;

            int value = p.photonView.GetHealth() + amount;

            value = Mathf.Min(value, p.MaxHealth);

            p.photonView.SetHealth(value);

            return true;
        }*/
    }
}
