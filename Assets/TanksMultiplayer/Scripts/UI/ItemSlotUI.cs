using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TanksMP;
using UnityEngine;

public class ItemSlotUI : UI<ItemSlotUI>
{
    public ItemData Data { get; set; }

    protected override void OnRefreshUI()
    {

    }

    public void OnClick()
    {
        var players = FindObjectsOfType<Player>();

        var myPlayer = players.FirstOrDefault(i => i.photonView.IsMine);

        var effect = (ItemEffectManager)Activator.CreateInstance(Type.GetType(Data.ClassName));

        effect.Execute(Data, myPlayer, myPlayer);
    }
}
