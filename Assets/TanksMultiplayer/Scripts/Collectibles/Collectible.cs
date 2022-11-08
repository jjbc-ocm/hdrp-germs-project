/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;
using Photon.Pun;

namespace TanksMP
{
    /// <summary>
    /// Base class for all derived Collectibles (health, shields, etc.) consumed or carried around.
    /// Extend this to create highly customized Collectible with specific functionality.
    /// </summary>
    /// 

    [RequireComponent(typeof(PhotonView))]
	public class Collectible : MonoBehaviourPun, IPunObservable
    {
        public AudioClip useClip;

        [SerializeField]
        private GameObject graphics;

        public virtual void OnTriggerEnter(Collider col)
		{
            if (!PhotonNetwork.IsMasterClient) return;
            
    		GameObject obj = col.gameObject;

			Player player = obj.GetComponent<Player>();

            if (Apply(player))
            {
                PhotonNetwork.Destroy(photonView);
            }
		}

        public virtual bool Apply(Player p)
		{
            return p != null;
        }


        

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {

        }
    }
}
