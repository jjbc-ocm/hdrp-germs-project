using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GP_DUMMY_PART_TYPE
{
    kSkin,
    kEye,
    kMouth,
    kHead,
    kWear,
    kGlove,
    kTail,
}

public enum GP_DUMMY_PART_RARITY
{
    kWood,
    kSilver,
    kGold,
    kCrystal
}

[CreateAssetMenu(fileName = "DummyPartSO", menuName = "ScriptableObjects/DummyPartSO")]
public class DummyPartSO : ScriptableObject
{
    [SerializeField]
    private int cost;

    [ShowAssetPreview]
    [SerializeField]
    private Sprite icon;

    /*[ShowAssetPreview]
    [SerializeField]
    private Sprite material;*/

    public GP_DUMMY_PART_TYPE m_type;
    public GP_DUMMY_PART_RARITY m_rarity;
    //[Tooltip("Name of the game object to activate in the dummy gameobject")]
    //public string m_gameObjectName; // All parts are already nested in the dummy gameobject, so we just find it and activate it.

    /*[ShowAssetPreview]
    public Sprite m_displayIcon; // Icon that shows in the customziation menu.

    [ShowAssetPreview]
    public Material m_material; // Override material to use for that body part*/

    public int Cost { get => cost; }

    public Sprite Icon { get => icon; }

    //public Sprite Material { get => material; }

    public GP_DUMMY_PART_TYPE Type { get => m_type; }
}
