using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GP_DUMMY_PART_TYPE
{
    kSkin,
    kEye,
    kMouth,
    kHair,
    kHorn,
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
    [ScriptableObjectId]
    private string id;

    [SerializeField]
    private int cost;

    public GP_DUMMY_PART_TYPE m_type;
    public GP_DUMMY_PART_RARITY m_rarity;
    [Tooltip("Name of the game object to activate in the dummy gameobject")]
    public string m_gameObjectName; // All parts are already nested in the dummy gameobject, so we just find it and activate it.
    public Sprite m_displayIcon; // Icon that shows in the customziation menu.
    public Material m_material; // Override material to use for that body part

    public string ID { get => id; }

    public int Cost { get => cost; }
}
