/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;

namespace TanksMP
{
	public class HealthCollectible : Collectible
    {
        protected override void OnObtain(PlayerManager player)
        {
            Debug.Log("Collectible Health");

            var amount = player.Stat.MaxHealth() / 2;

            player.Stat.AddHealth(amount);

            if (player.photonView.IsMine && !player.IsBot)
            {
                PopupManager.Instance.ShowHeal(amount, transform.position);
            }

            Debug.Log("Collectible Health End");
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
