using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor.Experimental.GraphView;

public class DummyCustomizeUI : ListViewUI<DummyCustomizeItemUI, DummyCustomizeUI>
{
    public List<DummyPartSO> Data { get; set; }

    #region Unity

    private void OnEnable()
    {
        Debug.Log("CALLED " + Data.Count);
        foreach (var data in Data)
        {
            Debug.Log(data.name);
        }
    }

    #endregion

    protected override void OnRefreshUI()
    {
        DeleteItems();

        RefreshItems(Data, (item, data) =>
        {
            item.Data = data;
        });
    }
}
