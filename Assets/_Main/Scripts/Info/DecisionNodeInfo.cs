using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionNodeInfo
{
    private Action decision;

    private float weight;

    public Action Decision { get => decision; set => decision = value; }

    public float Weight { get => weight; set => weight = value; }
}
