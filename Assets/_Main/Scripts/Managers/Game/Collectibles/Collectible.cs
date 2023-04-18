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

    }

    protected override void OnTriggerExitCalled(Collider col)
    {

    }

    public virtual bool Apply(PlayerManager p)
	{
        return p != null;
    }




    #region Photon

    [PunRPC]
    public void RpcDestroy()
    {
        PhotonNetwork.Destroy(photonView);
    }

    [PunRPC]
    public void RpcObtain(int viewID)
    {
        if (isObtained) return;

        var player = PhotonNetwork.GetPhotonView(viewID).GetComponent<PlayerManager>();

        OnObtain(player);

        isObtained = true;

        AudioManager.Instance.Play3D(sound, transform.position);

        photonView.RPC("RpcDestroy", RpcTarget.MasterClient);
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

    #endregion

    protected abstract void OnObtain(PlayerManager player);
}
