/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
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
        /*[PunRPC]
        public void RpcCollect()
        {
            PhotonNetwork.Destroy(photonView);
        }*/

        protected override void OnObtain(Player player)
        {
            player.photonView.HasChest(true);
        }

        /*public override void OnTriggerEnter(Collider col)
        {
            

            GameObject obj = col.gameObject;

            Player player = obj.GetComponent<Player>();

            if (Apply(player))
            {
                var view = player.photonView;

                player.photonView.RPC("RpcHasChest", RpcTarget.All, true);

                if (view.IsMine)
                {
                    var destination = view.GetTeam() == 0
                        ? GameManager.GetInstance().zoneRed.transform.position
                        : GameManager.GetInstance().zoneBlue.transform.position;

                    GPSManager.Instance.SetDestination(destination);
                }

                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Destroy(photonView);
                }
                
            }
        }*/
    }
}