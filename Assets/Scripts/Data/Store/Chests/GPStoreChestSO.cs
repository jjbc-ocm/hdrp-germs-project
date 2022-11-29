using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GP_CHEST_TAG
{
    kNone,
    kBest,
    kPopular
}

[CreateAssetMenu(fileName = "GPStoreChestSO", menuName = "ScriptableObjects/GPStoreChestSO")]
public class GPStoreChestSO : ScriptableObject
{
    public string m_chestName;
    public bool m_canBuyUsingGold = true;
    public int m_goldPrice;
    public bool m_canBuyUsingGems = true;
    public int m_gemPrice;
    public GP_CHEST_TAG m_specialTag;
    public Sprite m_chestIcon;
}
