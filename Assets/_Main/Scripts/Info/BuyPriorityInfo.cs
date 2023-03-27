using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuyPriorityInfo
{
    [SerializeField]
    private BotPropertyType property;

    [SerializeField]
    private bool isInverse;

    [SerializeField]
    private float weight;

    public BotPropertyType Property { get => property; }

    public bool IsInverse { get => isInverse; }

    public float Weight { get => weight; }
}
