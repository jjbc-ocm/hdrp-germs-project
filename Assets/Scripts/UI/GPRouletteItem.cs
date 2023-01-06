using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GPRouletteItem : MonoBehaviour
{
    public Image m_image;
    public TextMeshProUGUI m_amountText;

    public void SetSprite(Sprite sprite)
    {
        m_image.sprite = sprite;
    }

    public void SetTextAmount(int amount)
    {
        m_amountText.text = amount.ToString();
    }
}
