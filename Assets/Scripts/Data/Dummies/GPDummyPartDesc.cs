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

[CreateAssetMenu(fileName = "GPDummyPartDesc", menuName = "ScriptableObjects/GPDummyPartDesc")]
public class GPDummyPartDesc : ScriptableObject
{
    [SerializeField]
    [ScriptableObjectId]
    private string id;

    public GP_DUMMY_PART_TYPE m_type;
    [Tooltip("Name of the game object to activate in the dummy gameobject")]
    public string m_gameObjectName; // All parts are already nested in the dummy gameobject, so we just find it and activate it.
    public Sprite m_displayIcon; // Icon that shows in the customziation menu.
    public Material m_material; // Override material to use for that body part

    public string ID { get => id; }
}
