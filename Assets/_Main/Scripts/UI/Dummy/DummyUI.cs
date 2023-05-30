using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DummyUI : WindowUI<DummyUI>
{
    [SerializeField]
    private GPDummyLoader[] dummies;

    [SerializeField]
    private DummyCustomizeUI uiCustomize;

    public List<DummyData> Data { get; set; }

    public void OnDummyClick(int index)
    {
        var ownedItem = new List<DummyPartSO>();

        var items = APIManager.Instance.PlayerData.DummyParts;

        foreach (var part in items)
        {
            var rawData = JsonUtility.FromJson<DummyPartInstanceInfo>(part.InstanceData.GetAsString());

            var soData = SOManager.Instance.DummyParts.FirstOrDefault(i => i.name == rawData.name);

            if (soData)
            {
                ownedItem.Add(soData);
            }
        }

        uiCustomize.Open((self) =>
        {
            self.Data = ownedItem;

            self.DummyData = APIManager.Instance.PlayerData.Dummy(index);

            self.Index = index;
        });
    }

    public void OnBackClick()
    {
        uiCustomize.Close();
    }

    public void OnHomeClick()
    {
        HomeUI.Instance.Open();

        Close();
    }

    protected override void OnRefreshUI()
    {
        for (var i = 0; i < dummies.Length; i++)
        {
            dummies[i].ChangeAppearance(Data[i]);
        }
    }
}
