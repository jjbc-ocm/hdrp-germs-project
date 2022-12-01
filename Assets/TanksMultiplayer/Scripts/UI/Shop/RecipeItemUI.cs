using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeItemUI : UI<RecipeItemUI>
{
    [SerializeField]
    private Image imageSprite;

    [SerializeField]
    private TMP_Text textCost;

    public ItemData Data { get; set; }

    protected override void OnRefreshUI()
    {
        imageSprite.sprite = Data.Icon;

        textCost.text = Data.CostBuy.ToString();
    }
}
