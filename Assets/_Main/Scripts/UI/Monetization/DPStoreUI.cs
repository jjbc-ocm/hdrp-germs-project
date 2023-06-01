using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class DPStoreUI : ListViewUI<DPStoreItemUI, DPStoreUI>
{
    [SerializeField]
    private GPDummyLoader dummyPreview;

    [SerializeField]
    private Button buttonBuy;

    [SerializeField]
    private TMP_Text textBuy;

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

    private int selectedTabIndex;

    private DPStoreItemUI selectedItem;

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

    public void OnBuyClick()
    {
        ConfirmPurchaseUI.Instance.Open((self) =>
        {
            self.Header = selectedItem.Data.name;

            self.TextContent = string.Format("Buy this item for <color=#4BBAE0>{0} Gems</color>?", selectedItem.Data.Cost);

            self.IconContent = selectedItem.Data.Icon;

            self.OnConfirm = async () =>
            {
                SpinnerUI.Instance.Open();

                await APIManager.Instance.PlayerData.SubGems(selectedItem.Data.Cost);

                await APIManager.Instance.PlayerData.AddDummyPart(selectedItem.Data);

                StoreUI.Instance.RefreshUI();

                SpinnerUI.Instance.Close();

                self.Close();
            };
        });
        
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

                UpdateBuyButton();
            };
        });

        UpdateBuyButton();

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

    private void UpdateBuyButton()
    {
        var hasItem = APIManager.Instance.PlayerData.DummyParts
            .Select(part =>
            {
                var rawData = JsonUtility.FromJson<DummyPartInstanceInfo>(part.InstanceData.GetAsString());

                return SOManager.Instance.DummyParts.FirstOrDefault(i => i.name == rawData.name);
            })
            .Any(i => selectedItem && i == selectedItem.Data);

        buttonBuy.interactable = selectedItem != null && !hasItem;

        textBuy.text = !hasItem ? "Buy" : "Bought";
    }

    #endregion
}
