using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllItemsUI : UI<AllItemsUI>
{
    [SerializeField]
    private GameObject buttonSelectedIndicator;

    public bool IsSelected { get; set; }

    protected override void OnRefreshUI()
    {
        buttonSelectedIndicator.SetActive(IsSelected);
    }
}
