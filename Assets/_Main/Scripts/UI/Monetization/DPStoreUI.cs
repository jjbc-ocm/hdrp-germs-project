using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DPStoreUI : ListViewUI<DPStoreItemUI, DPStoreUI>
{
    [SerializeField]
    private GPDummyLoader dummyPreview;

    [SerializeField]
    private Button buttonBuy;

    [Header("Tab Settings")]

    [SerializeField]
    private Button[] buttonTabs;

    [SerializeField]
    private GP_DUMMY_PART_TYPE[] tabFilter;

    [SerializeField]
    private Transform tabSelectedIndicator;

    [SerializeField]
    private Color tabSelectedColor;

    [SerializeField]
    private Color tabNormalColor;

    private DPStoreItemUI selectedItem;

    private int selectedTabIndex;

    public DummyPartSO[] Data { get; set; }

    #region Unity

    private void OnEnable()
    {
        OnTabClick(0);
    }

    #endregion

    #region Public

    public void OnTabClick(int tabIndex)
    {
        selectedTabIndex = tabIndex;

        ResetTabUIs();

        UpdateTabUI(tabIndex, true);

        RefreshUI();
    }

    public async void OnBuyClick()
    {
        SpinnerUI.Instance.Open();

        await APIManager.Instance.PlayerData.SubGems(selectedItem.Data.Cost);

        await APIManager.Instance.PlayerData.AddDummyPart(selectedItem.Data);

        SpinnerUI.Instance.Close();
    }

    #endregion

    protected override void OnRefreshUI()
    {
        DeleteItems();

        RefreshItems(Data.Where(i => i.Type == tabFilter[selectedTabIndex]), (item, data) =>
        {
            item.Data = data;

            item.IsSelected = selectedItem?.Data == item.Data;

            item.OnClickCallback = () =>
            {
                selectedItem = item;

                RefreshUI();
            };
        });

        buttonBuy.interactable = selectedItem != null;

        if (selectedItem)
        {
            foreach (var part in SOManager.Instance.DummyParts)
            {
                dummyPreview.UnequipCustomPart(part.name);
            }

            dummyPreview.EquipCustomPart(selectedItem.Data.name);
        }
    }

    #region Private

    private void ResetTabUIs()
    {
        for (var i = 0; i < buttonTabs.Length; i++)
        {
            UpdateTabUI(i, false);
        }
    }

    private void UpdateTabUI(int tabIndex, bool isSelected)
    {
        buttonTabs[tabIndex].transform.GetChild(0).GetComponent<TMP_Text>().color = isSelected ? tabSelectedColor : tabNormalColor;

        if (isSelected)
        {
            LeanTween.move(tabSelectedIndicator.gameObject, buttonTabs[tabIndex].transform.position, 0.3f).setEaseSpring();
        }
    }

    #endregion
}
