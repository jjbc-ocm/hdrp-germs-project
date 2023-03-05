using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCountData
{
    private ItemSO item;

    private int count;

    public ItemSO Item { get => item; set => item = value; }

    public int Count { get => count; set => count = value; }
}
