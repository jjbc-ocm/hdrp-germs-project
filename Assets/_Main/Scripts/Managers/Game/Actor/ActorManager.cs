using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ActorManager : GameEntityManager
{
    [SerializeField]
    protected bool isMonster;

    [Header("Events")]
    public UnityEvent<int> onDieEvent;

    protected BotInfo botInfo;

    public bool IsMonster { get => isMonster; }

    public bool IsBot { get => botInfo != null; }

    public string GetName()
    {
        if (botInfo != null) return botInfo.Name;

        return photonView.Owner.GetName();
    }

    public int GetTeam()
    {
        if (botInfo != null) return botInfo.Team;

        return photonView.Owner.GetTeam();
    }

    public bool HasChest()
    {
        if (botInfo != null) return botInfo.HasChest;

        return photonView.Owner.HasChest();
    }

    public bool HasSurrendered()
    {
        if (botInfo != null) return botInfo.HasSurrendered;

        return photonView.Owner.HasSurrendered();
    }

    public void HasChest(bool value)
    {
        if (botInfo != null)
        {
            botInfo.HasChest = value;
        }
        else
        {
            photonView.Owner.HasChest(value);
        }
    }

    public void HasSurrendered(bool value)
    {
        if (botInfo != null)
        {
            botInfo.HasSurrendered = value;
        }
        else
        {
            photonView.Owner.HasSurrendered(value);
        }
    }

    [PunRPC]
    public abstract void RpcDamageHealth(int amount, int attackerId);
}
