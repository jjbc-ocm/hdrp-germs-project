using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TutorialInfo
{
    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private int eventIndexMin;

    [SerializeField]
    private int eventIndexMax;

    public GameObject Prefab { get => prefab; }

    public int EventIndexMin { get => eventIndexMin; }

    public int EventIndexMax { get => eventIndexMax; }
}
