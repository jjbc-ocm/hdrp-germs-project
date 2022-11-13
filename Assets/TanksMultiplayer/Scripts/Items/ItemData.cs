using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : ScriptableObject
{
    [SerializeField]
    private new string name;

    [SerializeField]
    [TextArea]
    private string desc;

    [SerializeField]
    private CategoryType category;

    [SerializeField]
    private int costBuy;

    [SerializeField]
    private int costSell;

    public string Name { get => name; }

    public string Desc { get => desc; }

    public CategoryType Category { get => category; }

    public int CostBuy { get => costBuy; }

    public int CostSell { get => costSell; }
}
