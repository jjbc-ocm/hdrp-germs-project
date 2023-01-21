using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GPRouletteItem : MonoBehaviour
{
    public Image m_image;
    public TextMeshProUGUI m_amountText;

    /// <summary>
    /// Sets the sprite to display on the roulette item.
    /// </summary>
    /// <param name="sprite"></param>
    public void SetSprite(Sprite sprite)
    {
        m_image.sprite = sprite;
    }

    /// <summary>
    /// Set the displayed amount to win in the roulette item.
    /// </summary>
    /// <param name="amount"></param>
    public void SetTextAmount(int amount)
    {
        m_amountText.text = amount.ToString();
    }
}
