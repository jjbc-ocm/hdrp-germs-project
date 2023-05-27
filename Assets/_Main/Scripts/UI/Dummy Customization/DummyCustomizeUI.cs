using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor.Experimental.GraphView;

public class DummyCustomizeUI : ListViewUI<DummyCustomizeItemUI, DummyCustomizeUI>
{
    [SerializeField]
    private GPDummyLoader dummy;

    public List<DummyPartSO> Data { get; set; }

    public DummyData DummyData { get; set; }

    public int Index { get;set; }

    #region Unity

    /*private void OnEnable()
    {
        Debug.Log("CALLED " + Data.Count);
        foreach (var data in Data)
        {
            Debug.Log(data.name);
        }
    }*/

    #endregion

    protected override void OnRefreshUI()
    {
        dummy.ChangeAppearance(DummyData);

        DeleteItems();

        RefreshItems(Data, (item, data) =>
        {
            item.OnClickCallback = async () =>
            {
                DummyData.AutoSetPart(data);

                SpinnerUI.Instance.Open();

                await APIManager.Instance.PlayerData.SetDummy(DummyData, Index).Put();

                SpinnerUI.Instance.Close();

                DummyUI.Instance.RefreshUI((self) =>
                {
                    self.Data = new List<DummyData> {
                        APIManager.Instance.PlayerData.Dummy(0),
                        APIManager.Instance.PlayerData.Dummy(1),
                        APIManager.Instance.PlayerData.Dummy(2)
                    };
                });

                RefreshUI((self) =>
                {
                    self.DummyData = DummyData;
                });
            };

            item.Data = data;
        });
    }
}
