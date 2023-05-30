using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class DummyCustomizeUI : ListViewUI<DummyCustomizeItemUI, DummyCustomizeUI>
{
    [SerializeField]
    private GPDummyLoader dummy;

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

    public List<DummyPartSO> Data { get; set; }

    public DummyData DummyData { get; set; }

    public int Index { get;set; }

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

    #endregion

    protected override void OnRefreshUI()
    {
        dummy.ChangeAppearance(DummyData);

        DeleteItems();

        RefreshItems(Data.Where(i => i.Type == tabFilter[selectedTabIndex]), (item, data) =>
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

            item.IsSelected = DummyData.Contains(data);
        });
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
