/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public abstract class Collectible : GameEntityManager, IPunObservable
{
    [SerializeField]
    private AudioClip sound;

    [SerializeField]
    private GameObject graphics;

    private bool isObtained;

    private void Update()
    {
        if (PlayerManager.Mine != null)
        {
            graphics.SetActive(IsVisibleRelativeTo(PlayerManager.Mine.GetTeam()));
        }
    }

    protected override void OnTriggerEnterCalled(Collider col)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        var obj = col.gameObject;

        var player = obj.GetComponent<PlayerManager>();

        if (Apply(player))
        {
            PhotonNetwork.Destroy(photonView);
        }
    }

    protected override void OnTriggerExitCalled(Collider col)
    {

    }

    public virtual bool Apply(PlayerManager p)
	{
        return p != null;
    }





    [PunRPC]
    public void Destroy()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        PhotonNetwork.Destroy(photonView);
    }

    public void Obtain(PlayerManager player)
    {
        OnObtain(player);

        graphics.SetActive(false);

        AudioManager.Instance.Play3D(sound, transform.position);

        photonView.RPC("Destroy", RpcTarget.MasterClient);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isObtained);
        }
        else
        {
            isObtained = (bool)stream.ReceiveNext();
        }
    }

    protected abstract void OnObtain(PlayerManager player);
}
