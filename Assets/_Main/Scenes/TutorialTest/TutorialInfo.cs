using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TutorialInfo
{
    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private int partFrom;

    [SerializeField]
    private int partTo;

    public GameObject Prefab { get => prefab; }

    public int PartFrom { get => partFrom; }

    public int PartTo { get => partTo; }
}
