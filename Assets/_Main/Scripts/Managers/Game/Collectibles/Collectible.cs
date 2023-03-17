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
	public abstract class Collectible : GameEntityManager, IPunObservable
    {
        [SerializeField]
        private AudioClip sound;

        [SerializeField]
        private GameObject graphics;

        void Update()
        {
            if (Player.Mine != null)
            {
                graphics.SetActive(IsVisibleRelativeTo(Player.Mine.transform));
            }
        }

        protected override void OnTriggerEnterCalled(Collider col)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            GameObject obj = col.gameObject;

            Player player = obj.GetComponent<Player>();

            if (Apply(player))
            {
                PhotonNetwork.Destroy(photonView);
            }
        }

        protected override void OnTriggerExitCalled(Collider col)
        {

        }

        public virtual bool Apply(Player p)
		{
            return p != null;
        }





        [PunRPC]
        public void Destroy()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            PhotonNetwork.Destroy(photonView);
        }

        public void Obtain(Player player)
        {
            OnObtain(player);

            graphics.SetActive(false);

            AudioManager.Instance.Play3D(sound, transform.position);

            photonView.RPC("Destroy", RpcTarget.MasterClient);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {

        }

        protected abstract void OnObtain(Player player);
    }
}
