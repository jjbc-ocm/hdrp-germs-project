using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreUI : UI<StoreUI>
{
    [SerializeField]
    private GemsUI uiGems;



    public void OnGemsClick()
    {
        SpinnerUI.Instance.Open();

        IAPManager.Instance.InitializeShopItems((items) =>
        {
            uiGems.Open((self) =>
            {
                self.Data = items;
            });

            SpinnerUI.Instance.Close();
        });
        
    }

    protected override void OnRefreshUI()
    {

    }
}
