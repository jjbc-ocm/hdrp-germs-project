
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