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

    private int statHealth;
    private int statMana;

    private int inventoryGold;
    private string inventoryItemId0;
    private string inventoryItemId1;
    private string inventoryItemId2;
    private string inventoryItemId3;
    private string inventoryItemId4;
    private string inventoryItemId5;

    public Player Player { get => player; }

    public string UserId { get => userId; }

    public int ShipIndex { get => shipIndex; }

    public void Initialize(Player player, bool isApplyStateToPlayer)
    {
        this.player = player;

        userId = player.photonView.Owner.UserId;

        shipIndex = player.photonView.Owner.GetShipIndex();

        if (isApplyStateToPlayer)
        {
            player.transform.position = transformPosition;

            player.transform.rotation = transformRotation;

            player.Stat.SetHealth(statHealth);

            player.Stat.SetMana(statMana);

            player.Inventory.Gold = inventoryGold;

            player.Inventory.ItemId0 = inventoryItemId0;

            player.Inventory.ItemId1 = inventoryItemId1;

            player.Inventory.ItemId2 = inventoryItemId2;

            player.Inventory.ItemId3 = inventoryItemId3;

            player.Inventory.ItemId4 = inventoryItemId4;

            player.Inventory.ItemId5 = inventoryItemId5;
        }
    }

    void Update()
    {
        transformPosition = player.transform.position;

        transformRotation = player.transform.rotation;

        statHealth = player.Stat.Health;

        statMana = player.Stat.Mana;

        inventoryGold = player.Inventory.Gold;

        inventoryItemId0 = player.Inventory.ItemId0;

        inventoryItemId1 = player.Inventory.ItemId1;

        inventoryItemId2 = player.Inventory.ItemId2;

        inventoryItemId3 = player.Inventory.ItemId3;

        inventoryItemId4 = player.Inventory.ItemId4;

        inventoryItemId5 = player.Inventory.ItemId5;
    }
}
