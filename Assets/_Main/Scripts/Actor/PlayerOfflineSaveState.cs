using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class PlayerOfflineSaveState : MonoBehaviour
{
    private Player player;

    private string userId;

    private int shipIndex;

    private Vector3 transformPosition;
    private Quaternion transformRotation;

    //private Vector3 shipRotation;

    /*private int statAttackDamage;
    private int statAbilityPower;
    private int statArmor;
    private int statResist;
    private int statAttackSpeed;
    private int statMoveSpeed;
    private int statHealth;
    private int statMana;*/

    public Player Player { get => player; }

    public string UserId { get => userId; }

    public int ShipIndex { get => shipIndex; }

    public Vector3 TransformPosition { get => transformPosition; }

    public Quaternion TransformRotation { get => transformRotation; }

    public void Initialize(Player player, bool isApplyStateToPlayer)
    {
        this.player = player;

        userId = player.photonView.Owner.UserId;

        shipIndex = player.photonView.Owner.GetShipIndex();

        if (isApplyStateToPlayer)
        {
            player.transform.position = transformPosition;

            player.transform.rotation = transformRotation;
        }
    }

    void Update()
    {
        transformPosition = player.transform.position;

        transformRotation = player.transform.rotation;
    }
}
