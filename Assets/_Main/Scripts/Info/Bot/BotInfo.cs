using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BotInfo
{
    [SerializeField]
    private string name;

    [SerializeField]
    private int team;

    [SerializeField]
    private int shipIndex;

    [SerializeField]
    private bool hasChest;

    [SerializeField]
    private bool hasSurrendered;

    public string Name { get => name; set => name = value; }

    public int Team { get => team; set => team = value; }

    public int ShipIndex { get => shipIndex; set => shipIndex = value; }

    public bool HasChest { get => hasChest; set => hasChest = value; }

    public bool HasSurrendered { get => hasSurrendered; set => hasSurrendered = value; }
}
