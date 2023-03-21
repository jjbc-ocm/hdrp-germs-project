using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectShipUI : WindowListViewUI<GPShipCard, SelectShipUI>
{
    protected override void OnRefreshUI()
    {
        DeleteItems();

        RefreshItems(SOManager.Instance.PlayerShips, (item, data) =>
        {
            item.Data = data;

            item.OnClickCallback = () =>
            {
                PhotonNetwork.LocalPlayer.SetShipIdx(data.m_prefabListIndex);
            };
        });
    }
}

