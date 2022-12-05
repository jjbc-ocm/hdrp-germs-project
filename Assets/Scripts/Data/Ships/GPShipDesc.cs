using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TanksMP;

public enum GP_SHIP_TYPE
{
    kDark = 0,
    kMagical,
    kNatural,
    kLight,
}

[CreateAssetMenu(fileName = "GPShipDesc", menuName = "ScriptableObjects/GPShipDesc")]
public class GPShipDesc : ScriptableObject
{
    [SerializeField]
    [ScriptableObjectId]
    private string id;

    [Tooltip("Must match the one on the GameNetworkManager shipPrefabs list")]
    public int m_prefabListIndex = 0;
    public string m_name;
    [TextArea(2, 10)]
    public string m_desc;
    [TextArea(2, 10)]
    public string m_ability;
    public GameObject m_model;
    public Player m_playerPrefab;
    public Sprite m_cardImage;
    public GP_SHIP_TYPE m_type;

    [SerializeField]
    private List<ItemData> idealStarterItems;

    [SerializeField]
    private List<ItemData> idealOffensiveItems;

    [SerializeField]
    private List<ItemData> idealDefensiveItems;

    [SerializeField]
    private List<ItemData> idealUtilityItems;

    public string ID { get => id; }

    public List<ItemData> IdealStarterItems { get => idealStarterItems; }

    public List<ItemData> IdealOffensiveItems { get => idealOffensiveItems; }

    public List<ItemData> IdealDefensiveItems { get => idealDefensiveItems; }

    public List<ItemData> IdealUtilityItems { get => idealUtilityItems; }
}
