/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;
using Photon.Pun;

namespace TanksMP
{
    /// <summary>
    /// Custom Collectible implementation for scene owned (unassigned) or team owned items.
    /// E.g. allowing for 'Rambo' pickups, Capture the Flag items etc.
    /// </summary>
	public class CollectibleTeam : Collectible
    {


        /// <summary>
        /// Server only: check for players colliding with the powerup.
        /// Possible collision are defined in the Physics Matrix.
        /// TODO: this is old implementation
        /// </summary>
        /*public override void OnTriggerEnter(Collider col)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            GameObject obj = col.gameObject;
            Player player = obj.GetComponent<Player>();

            //try to apply collectible to player, the result should be true
            if (Apply(player))
            {
                //clean up previous buffered RPCs so we only keep the most recent one
                PhotonNetwork.RemoveRPCs(spawner.photonView);

                //player picked up item from other team, send out buffered RPC for it to be remembered
                spawner.photonView.RPC("Pickup", RpcTarget.AllBuffered, (short)player.GetView().ViewID);
            }
        }*/

        public override void OnTriggerEnter(Collider col)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            GameObject obj = col.gameObject;
            Player player = obj.GetComponent<Player>();

            if (Apply(player))
            {
                //clean up previous buffered RPCs so we only keep the most recent one
                //PhotonNetwork.RemoveRPCs(spawner.photonView);

                //player picked up item from other team, send out buffered RPC for it to be remembered
                //spawner.photonView.RPC("Pickup", RpcTarget.AllBuffered, (short)player.GetView().ViewID);

                var view = player.GetView();

                carrierId = view.ViewID;

                //GameManager.GetInstance().ui.OnChestPickup(view.GetComponent<Player>()); // TODO: return this later

                

                if (view.IsMine)
                {
                    Debug.Log("EEEEEEEE");

                    var destination = view.GetTeam() == 0
                        ? GameManager.GetInstance().zoneRed.transform.position
                        : GameManager.GetInstance().zoneBlue.transform.position;

                    GPSManager.Instance.SetDestination(destination);
                }

                //OnPickup();
            }
        }


        /// <summary>
        /// Overrides the default behavior with a custom implementation.
        /// Check for the carrier and item position to decide valid pickup.
        /// </summary>
        public override bool Apply(Player p)
        {
            /* Cannot collect this item if collider is not a player or already carried by other player */
            if (p == null ||  carrierId > 0)
                return false;

            //if a target renderer is set, assign team material
            //Colorize(p.GetView().GetTeam());

            //return successful collection
            return true;
        }

        /// <summary>
        /// Implemented by: Jilmer John Cariaso
        /// </summary>
        /*public override void OnPickup()
        {
            //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;

        }*/

        /// <summary>
        /// Overrides the default behavior with a custom implementation.
        /// </summary>
        public override void OnDrop()
        {
            //Colorize(this.teamIndex);

            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }


        /// <summary>
        /// Overrides the default behavior with a custom implementation.
        /// </summary>
        public override void OnReturn()
        {
            //Colorize(this.teamIndex);
        }


        //assign material based on team index passed in
        /*void Colorize(int teamIndex)
        {
            if (targetRenderer != null)
            {
                if (teamIndex >= 0)
                    targetRenderer.material = GameManager.GetInstance().teams[teamIndex].material;
                else
                    targetRenderer.material = baseMaterial;
            }
        }*/
    }
}