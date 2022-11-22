using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoItemsUI : UI<RecoItemsUI>
{
    [SerializeField]
    private GameObject buttonSelectedIndicator;

    public bool IsSelected { get; set; }

    protected override void OnRefreshUI()
    {
        buttonSelectedIndicator.SetActive(IsSelected);
    }
}
