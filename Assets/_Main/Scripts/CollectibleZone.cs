/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

namespace TanksMP
{
    /// <summary>
    /// Component that acts as an area to trigger collection of CollectibleTeam items.
    /// E.g. necessary for team bases in Capture The Flag mode. Needs a Collider to trigger.
    /// </summary>
	public class CollectibleZone : GameEntityManager
    {
        [SerializeField]
        private int team;

        [SerializeField]
        private AudioClip clip;

        public int Team { get => team; }

        public AudioClip Clip { get => clip; }

        public void OnDropChest()
        {
            if (clip) AudioManager.Instance.Play3D(clip, transform.position);

            GameManager.Instance.AddScore(ScoreType.Capture, team);

            if (GameManager.Instance.IsGameOver(out List<BattleResultType> teamResults))
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;

                Player.Mine.photonView.RPC("RpcGameOver", RpcTarget.All, teamResults.IndexOf(BattleResultType.Victory));

                return;
            }
        }

        protected override void OnTriggerEnterCalled(Collider col)
        {

        }

        protected override void OnTriggerExitCalled(Collider col)
        {

        }
    }
}