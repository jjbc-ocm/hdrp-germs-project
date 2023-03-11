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

    public int Index { get; set; }

    public ItemSO Data { get; set; }

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
                ui.SelectedData = null;

                ui.SelectedSlotIndex = Index;
            });
        }
        else
        {
            if (Data == null || string.IsNullOrEmpty(Data.ClassName)) return;

            var effect = (ItemEffectManager)Activator.CreateInstance(Type.GetType(Data.ClassName));

            effect.Execute(Index, Player.Mine);
        }
    }
}
