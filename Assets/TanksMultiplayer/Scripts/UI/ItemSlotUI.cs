using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TanksMP;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : UI<ItemSlotUI>
{
    [SerializeField]
    private Image imageSprite;

    public ItemData Data { get; set; }

    protected override void OnRefreshUI()
    {
        imageSprite.gameObject.SetActive(Data != null);

        if (Data != null)
        {
            imageSprite.sprite = Data.Icon;
        }
    }

    public void OnClick()
    {
        if (ShopManager.Instance.UI.gameObject.activeSelf)
        {
            ShopManager.Instance.UI.RefreshUI((ui) =>
            {
                ui.Selected = Data;
            });
        }
        else
        {
            if (string.IsNullOrEmpty(Data.ClassName)) return;

            var effect = (ItemEffectManager)Activator.CreateInstance(Type.GetType(Data.ClassName));

            effect.Execute(Data, Player.Mine, Player.Mine.transform.position);
        }
    }
}
