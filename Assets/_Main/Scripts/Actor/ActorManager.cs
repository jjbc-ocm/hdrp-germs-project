using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ActorManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private bool isMonster;

    [Header("Events")]
    public UnityEvent<int> onDieEvent;

    public bool IsMonster { get => isMonster; }

    [PunRPC]
    public abstract void RpcDamageHealth(int amount, int attackerId);
}
