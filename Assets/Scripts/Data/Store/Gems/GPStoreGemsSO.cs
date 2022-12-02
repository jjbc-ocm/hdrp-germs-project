using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GP_GEM_PACK_TAG
{
    kNone,
    kBest,
    kPopular
}

[CreateAssetMenu(fileName = "GPStoreGemsSO", menuName = "ScriptableObjects/GPStoreGemsSO")]
public class GPStoreGemsSO : ScriptableObject
{
    public string m_packName;
    public float m_usdPrice;
    public int m_gemAmount;
    public GP_GEM_PACK_TAG m_specialTag;

    [Header("Icon settings")]
    public Sprite m_gemIcon;
    public bool m_overrideSize = false;
    public Vector2 m_sizeOverride;
    public bool m_overridePosition = false;
    public Vector2 m_posOverride;
}
