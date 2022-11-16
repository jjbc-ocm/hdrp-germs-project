using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GPShipCard : MonoBehaviour
{
    public Image m_shipImage;
    public Image m_typeImage;
    public TextMeshProUGUI m_nameText;

    public void SetShipImage(Sprite sprite)
    {
        m_shipImage.sprite = sprite;
    }

    public void SetTypeImage(Sprite sprite)
    {
        m_typeImage.sprite = sprite;
    }

    public void SetName(string name)
    {
        m_nameText.text = name;
    }
}
