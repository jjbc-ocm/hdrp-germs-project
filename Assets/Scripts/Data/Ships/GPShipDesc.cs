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
    public PlayerManager m_playerPrefab;
    public Sprite m_cardImage;

    [SerializeField]
    private Sprite m_shipIconImage;

    public GP_SHIP_TYPE m_type;

    [SerializeField]
    private List<ItemSO> idealStarterItems;

    [SerializeField]
    private List<ItemSO> idealOffensiveItems;

    [SerializeField]
    private List<ItemSO> idealDefensiveItems;

    [SerializeField]
    private List<ItemSO> idealUtilityItems;

    [SerializeField]
    private PersonalityInfo personality;

    public string ID { get => id; }

    public Sprite ShipIconImage { get => m_shipIconImage; }

    public List<ItemSO> IdealStarterItems { get => idealStarterItems; }

    public List<ItemSO> IdealOffensiveItems { get => idealOffensiveItems; }

    public List<ItemSO> IdealDefensiveItems { get => idealDefensiveItems; }

    public List<ItemSO> IdealUtilityItems { get => idealUtilityItems; }

    public PersonalityInfo Personality { get => personality; }
}
