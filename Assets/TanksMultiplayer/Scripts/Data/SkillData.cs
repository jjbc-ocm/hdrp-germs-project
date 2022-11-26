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
    private Sprite icon;

    [SerializeField]
    private AimType aim;

    [SerializeField]
    private TargetType target;

    [SerializeField]
    private float range;

    [SerializeField]
    private int mpCost;

    [SerializeField]
    private bool isSpawnOnAim;

    [SerializeField]
    private SkillBaseManager effect;

    public string Name { get => name; }

    public string Desc { get => desc; }

    public Sprite Icon { get => icon; }

    public AimType Aim { get => aim; }

    public TargetType Target { get => target; }

    public float Range { get => range; }

    public int MpCost { get => mpCost; }

    public bool IsSpawnOnAim { get => isSpawnOnAim; }

    public SkillBaseManager Effect { get => effect; }
}
