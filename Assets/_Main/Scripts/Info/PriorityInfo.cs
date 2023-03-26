using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PriorityInfo
{
    [SerializeField]
    private string key;

    [SerializeField]
    private BotTargetType target;

    [SerializeField]
    private BotPropertyType property;

    [SerializeField]
    private bool isInverse;

    [SerializeField]
    private float weight;

    public string Key { get => key; }

    public BotTargetType Target { get => target; }

    public BotPropertyType Property { get => property; }

    public bool IsInverse { get => isInverse; }

    public float Weight { get => weight; }
}