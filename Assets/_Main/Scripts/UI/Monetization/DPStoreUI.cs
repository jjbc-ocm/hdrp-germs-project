using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DPStoreUI : ListViewUI<DPStoreItemUI, DPStoreUI>
{
    [SerializeField]
    private Button buttonBuy;

    private DPStoreItemUI selectedItem;

    public DummyPartSO[] Data { get; set; }

    public async void OnBuyClick()
    {
        SpinnerUI.Instance.Open();

        await APIManager.Instance.PlayerData.AddDummyPart(selectedItem.Data);

        SpinnerUI.Instance.Close();
    }

    protected override void OnRefreshUI()
    {
        DeleteItems();

        RefreshItems(Data, (item, data) =>
        {
            item.Data = data;

            item.OnClickCallback = () =>
            {
                selectedItem = item;

                RefreshUI();
            };
        });

        //buttonBuy.interactable = selectedItem != null;
    }
}
