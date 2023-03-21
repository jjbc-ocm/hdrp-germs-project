using System.Collections;
using System.Collections.Generic;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeItemUI : UI<RecipeItemUI>
{
    [SerializeField]
    private Image imageSprite;

    [SerializeField]
    private TMP_Text textCost;

    public ItemSO Data { get; set; }

    protected override void OnRefreshUI()
    {
        imageSprite.sprite = Data.Icon;

        textCost.text = ShopManager.Instance.GetTotalCost(PlayerManager.Mine, Data).ToString();
    }

    public void OnClick()
    {
        ShopManager.Instance.UI.RefreshUI((ui) =>
        {
            ui.SelectedData = Data;

            ui.SelectedSlotIndex = -1;
        });
    }
}
