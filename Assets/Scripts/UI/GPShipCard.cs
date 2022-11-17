using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class GPShipCard : MonoBehaviour
{
    public Image m_shipImage;
    public Image m_typeImage;
    public TextMeshProUGUI m_nameText;
    public List<Sprite> m_shipTypeImages;
    public UnityEvent<GPShipCard> OnCardClickedEvent;
    GPShipDesc m_shipDesc;
    public GPShipDesc m_ShipDesc { get => m_shipDesc; }

    public void DisplayShipDesc(GPShipDesc desc)
    {
        m_shipDesc = desc;
        SetShipImage(desc.m_cardImage);
        SetName(desc.m_name);
        SetTypeImage(m_shipTypeImages[(int)desc.m_type]);
    }

    void SetShipImage(Sprite sprite)
    {
        m_shipImage.sprite = sprite;
    }

    void SetTypeImage(Sprite sprite)
    {
        m_typeImage.sprite = sprite;
    }

    void SetName(string name)
    {
        m_nameText.text = name;
    }

    public void OnCardClicked()
    {
        if (OnCardClickedEvent != null)
        {
            OnCardClickedEvent.Invoke(this);
        }
    }
}
