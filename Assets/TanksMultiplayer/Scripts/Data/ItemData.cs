using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ScriptableObjectIdAttribute : PropertyAttribute { }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ScriptableObjectIdAttribute))]
public class ScriptableObjectIdDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        if (string.IsNullOrEmpty(property.stringValue))
        {
            property.stringValue = Guid.NewGuid().ToString();
        }
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
#endif

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    [SerializeField]
    [ScriptableObjectId]
    private string id;

    [SerializeField]
    private new string name;

    [SerializeField]
    [TextArea(5, 99)]
    private string desc;

    [SerializeField]
    private Sprite icon;

    [SerializeField]
    private CategoryType category;

    [SerializeField]
    private int costBuy;

    [SerializeField]
    private int costSell;

    [SerializeField]
    private string className;

    [SerializeField]
    private StatModifierData statModifier;

    [SerializeField]
    private float duration;

    public string ID { get => id; }

    public string Name { get => name; }

    public string Desc { get => desc; }

    public Sprite Icon { get => icon; }

    public CategoryType Category { get => category; }

    public int CostBuy { get => costBuy; }

    public int CostSell { get => costSell; }

    public string ClassName { get => className; }

    public StatModifierData StatModifier { get => statModifier; }

    public float Duration { get => duration; }
}
