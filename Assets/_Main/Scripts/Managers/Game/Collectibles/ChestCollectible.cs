/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using System.Collections;

public class ChestCollectible : Collectible
{
    [SerializeField]
    private int team;

    private void Start()
    {
        StartCoroutine(YieldAutoReturn());
    }

    protected override void OnObtain(PlayerManager player)
    {
        player.Stat.SetChest(true, team);

        GuideManager.Instance.TryAddChestGuide();
    }

    private IEnumerator YieldAutoReturn()
    {
        yield return new WaitForSeconds(SOManager.Instance.Constants.ReturnChestTime);

        photonView.RPC("RpcDestroy", RpcTarget.MasterClient);
    }
}