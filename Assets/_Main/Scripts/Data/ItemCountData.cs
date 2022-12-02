using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCountData
{
    private ItemData item;

    private int count;

    public ItemData Item { get => item; set => item = value; }

    public int Count { get => count; set => count = value; }
}
