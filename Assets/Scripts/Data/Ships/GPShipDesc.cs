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
}
