using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class DummyCustomizeUI : UI<DummyCustomizeUI>
{
    private void OnEnable()
    {
        var items = APIManager.Instance.PlayerData.DummyParts;

        foreach (var part in items)
        {
            Debug.Log(part.InstanceData.GetAsString());
        }
    }

    protected override void OnRefreshUI()
    {

    }
}
