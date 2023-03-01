using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionNodeInfo
{
    private string key;

    private Action decision;

    private float weight;

    public string Key { get => key; set => key = value; }

    public Action Decision { get => decision; set => decision = value; }

    public float Weight { get => weight; set => weight = value; }
}
