using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SkillData : ScriptableObject
{
    [SerializeField]
    private new string name;

    [SerializeField]
    [TextArea]
    private string desc;

    [SerializeField]
    private AimType aim;

    [SerializeField]
    private TargetType target;

    [SerializeField]
    private int mpCost;

    [SerializeField]
    private GameObject effect;

    public string Name { get => name; }

    public string Desc { get => desc; }

    public AimType Aim { get => aim; }

    public TargetType Target { get => target; }

    public int MpCost { get => mpCost; }

    public GameObject Effect { get => effect; }
}
