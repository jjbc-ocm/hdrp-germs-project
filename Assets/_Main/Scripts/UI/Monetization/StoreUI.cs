using Org.BouncyCastle.Asn1.Mozilla;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreUI : WindowUI<StoreUI>
{
    [SerializeField]
    private CurrencyUI uiCurrency;

    [SerializeField]
    private GemsUI uiGems;

    [SerializeField]
    private DPStoreUI uiDPStore;

    [Header("Tab Settings")]

    [SerializeField]
    private Button[] buttonTabs;

    [SerializeField]
    private Transform tabSelectedIndicator;

    [SerializeField]
    private Color tabSelectedColor;

    [SerializeField]
    private Color tabNormalColor;

    #region Unity

    private void OnEnable()
    {
        OnGemsClick(buttonTabs[0]);
    }

    #endregion

    #region Public

    public void OnGemsClick(Button buttonSelf)
    {
        ResetTabUIs();

        SpinnerUI.Instance.Open();

        IAPManager.Instance.InitializeShopItems((items) =>
        {
            uiGems.Open((self) =>
            {
                self.Data = items;
            });

            SpinnerUI.Instance.Close();

            UpdateTabUI(buttonSelf, true);
        });

    }

    public void OnDummyPartStoreClick(Button buttonSelf)
    {
        ResetTabUIs();

        uiDPStore.Open((self) =>
        {
            self.Data = SOManager.Instance.DummyParts;
        });

        UpdateTabUI(buttonSelf, true);
    }

    public void OnHomeClick()
    {
        HomeUI.Instance.Open();

        Close();
    }

    #endregion

    protected override void OnRefreshUI()
    {
        uiCurrency.RefreshUI((self) =>
        {
            self.Gems = APIManager.Instance.PlayerData.Gems;
        });
    }

    #region Private

    private void ResetTabUIs()
    {
        foreach (var buttonTab in buttonTabs)
        {
            UpdateTabUI(buttonTab, false);
        }

        uiGems.Close();

        uiDPStore.Close();
    }

    private void UpdateTabUI(Button buttonSelf, bool isSelected)
    {
        buttonSelf.GetComponent<TMP_Text>().color = isSelected ? tabSelectedColor : tabNormalColor;

        if (isSelected)
        {
            LeanTween.move(tabSelectedIndicator.gameObject, buttonSelf.transform.position, 0.3f).setEaseSpring();
        }
    }

    #endregion
}
