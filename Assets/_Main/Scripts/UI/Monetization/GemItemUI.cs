using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GemItemUI : UI<GemItemUI>
{
    [SerializeField]
    private GemSO data;

    [SerializeField]
    private TMP_Text textName;

    [SerializeField]
    private TMP_Text textAmount;

    [SerializeField]
    private TMP_Text textCost;

    public InventoryDef Data { get; set; }

    public void OnBuyClick()
    {
        ConfirmPurchaseUI.Instance.Open((self) =>
        {
            self.Header = data.name;

            self.TextContent = string.Format("Buy <color=#4BBAE0>{0} Gems</color> for {1}?", data.Amount, Data.LocalPriceFormatted);

            self.IconContent = data.Sprite;

            self.OnConfirm = () =>
            {
                SpinnerUI.Instance.Open();

                IAPManager.Instance.BuyGem(Data, async () =>
                {
                    await APIManager.Instance.PlayerData.AddGems(data.Amount);

                    SpinnerUI.Instance.Close();
                });

                self.Close();
            };
        });

       
    }

    protected override void OnRefreshUI()
    {
        textName.text = data.name;

        textAmount.text = data.Amount.ToString();

        textCost.text = Data.LocalPriceFormatted;
    }
}
