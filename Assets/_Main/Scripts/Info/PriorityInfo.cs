using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PriorityInfo
{
    [SerializeField]
    private string key;

    [SerializeField]
    private float weight;

    public string Key { get => key; }

    public float Weight { get => weight; }
}