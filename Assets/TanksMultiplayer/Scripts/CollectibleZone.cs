/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;
using Photon.Pun;

namespace TanksMP
{
    /// <summary>
    /// Component that acts as an area to trigger collection of CollectibleTeam items.
    /// E.g. necessary for team bases in Capture The Flag mode. Needs a Collider to trigger.
    /// </summary>
	public class CollectibleZone : MonoBehaviour
    {
        /// <summary>
        /// Team index this zone belongs to.
        /// Teams are defined in the GameManager script inspector.
        /// </summary>
        public int teamIndex = 0;

        /// <summary>
        /// Clip to play when a CollectibleTeam item is brought to this zone.
        /// </summary>
        public AudioClip scoreClip;

        public void OnDropChest()
        {
            if (scoreClip) AudioManager.Play3D(scoreClip, transform.position);

            GameManager.GetInstance().AddScore(ScoreType.Capture, teamIndex);

            if (GameManager.GetInstance().IsGameOver())
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;

                Player.Mine.photonView.RPC("RpcGameOver", RpcTarget.All, (byte)teamIndex);

                return;
            }
        }

        // TODO: need to revise implementation
        // Why? It will only work if there's only 1 chest instance on the screen
        /*public void OnTriggerEnter(Collider col)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            if (GameManager.GetInstance().IsGameOver()) return;

            var player = col.GetComponent<Player>();

            if (player == null) return;

            if (player != null && player.photonView.GetTeam() != teamIndex) return;

            if (!player.HasChest) return;

            if (scoreClip) AudioManager.Play3D(scoreClip, transform.position);

            GameManager.GetInstance().AddScore(ScoreType.Capture, teamIndex);

            if (player.photonView.IsMine)
            {
                player.HasChest = false;
            }

            if (GameManager.GetInstance().IsGameOver())
            {
                //close room for joining players
                PhotonNetwork.CurrentRoom.IsOpen = false;
                //tell all clients the winning team
                GameManager.GetInstance().localPlayer.photonView.RPC("RpcGameOver", RpcTarget.All, (byte)teamIndex);
                return;
            }

            //PhotonNetwork.Destroy(chest.photonView);
        }*/
    }
}