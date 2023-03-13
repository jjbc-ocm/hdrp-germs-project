using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrepShipUI : UI<PrepShipUI>
{
    [SerializeField]
    private TMP_Text textName;

    [SerializeField]
    private Image imageShip;

    public Player Data { get; set; }

    protected override void OnRefreshUI()
    {
        textName.gameObject.SetActive(Data != null);

        imageShip.gameObject.SetActive(Data != null);

        if (Data != null)
        {
            textName.text = Data.GetName();

            imageShip.sprite = SOManager.Instance.PlayerShips.FirstOrDefault(i => i.m_prefabListIndex == Data.GetShipIndex())?.ShipIconImage ?? null;
        }
    }
}
