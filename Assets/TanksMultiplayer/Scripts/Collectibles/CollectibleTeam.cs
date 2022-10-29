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
    public UnityEvent<Player, GameObject> OnCollectedEvent;

    public override void OnTriggerEnter(Collider col)
    {
      if (!PhotonNetwork.IsMasterClient)
        return;

      GameObject obj = col.gameObject;
      Player player = obj.GetComponent<Player>();

      if (Apply(player))
      {
        var view = player.photonView;

        carrierId = view.ViewID;

        if (view.IsMine)
        {
          var destination = view.GetTeam() == 0
              ? GameManager.GetInstance().zoneRed.transform.position
              : GameManager.GetInstance().zoneBlue.transform.position;

          GPSManager.Instance.SetDestination(destination);
        }

        //OnPickup();
        if (OnCollectedEvent != null)
        {
          OnCollectedEvent.Invoke(player, obj);
        }
      }
    }

    public override bool Apply(Player p)
    {
      /* Cannot collect this item if collider is not a player or already carried by other player */
      if (p == null || carrierId > 0)
        return false;

      return true;
    }
    public override void OnDrop()
    {
      GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }

    public override void OnReturn()
    {

    }
  }
}