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

    /// <summary>
    /// Displays the ship image, ship name and ship type on the card using the given GPShipDesc.
    /// </summary>
    /// <param name="desc"></param>
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

    /// <summary>
    /// Linked to the OnClick event of the button component of the card.
    /// Calls OnCardClickedEvent to which you can suscribe to handle what you need.
    /// </summary>
    public void OnCardClicked()
    {
        if (OnCardClickedEvent != null)
        {
            OnCardClickedEvent.Invoke(this);
        }
    }
}
