using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class GPStoreChestCard : MonoBehaviour
{
    public Image m_iconImage;
    public TextMeshProUGUI m_chestName;
    public Button m_goldBuyButton;
    public Button m_gemBuyButton;
    public TextMeshProUGUI m_goldPriceText;
    public TextMeshProUGUI m_gemPriceText;
    public GameObject m_bestTag;
    public GameObject m_popularTag;
    public GPStoreChestSO m_chestDesc;
    public GPPunchTween m_punchTween;

    public TextMeshProUGUI m_contentDescText1;
    public TextMeshProUGUI m_contentDescText2;

    public UnityEvent<GPStoreChestCard> OnClickedBuyUsingGoldEvent;
    public UnityEvent<GPStoreChestCard> OnClickedBuyUsingGemsEvent;
    public UnityEvent<GPStoreChestCard> OnSuccesfullBuyEvent;

    // Start is called before the first frame update
    void Start()
    {
        m_goldBuyButton.onClick.AddListener(OnBuyUsingGold);
        m_gemBuyButton.onClick.AddListener(OnBuyUsingGems);

        if (m_chestDesc != null)
        {
            DisplayChest(m_chestDesc);
        }
    }

    /// <summary>
    /// Displays the chest image, anme, price, tag, content text using the given GPStoreChestSO.
    /// </summary>
    /// <param name="chestDesc"></param>
    public void DisplayChest(GPStoreChestSO chestDesc)
    {
        m_chestDesc = chestDesc;
        m_iconImage.sprite = chestDesc.m_chestIcon;
        m_chestName.text = chestDesc.m_chestName;
        m_goldPriceText.text = chestDesc.m_goldPrice.ToString();
        m_gemPriceText.text = chestDesc.m_gemPrice.ToString();
        m_goldBuyButton.gameObject.SetActive(chestDesc.m_canBuyUsingGold);
        m_gemPriceText.gameObject.SetActive(chestDesc.m_canBuyUsingGems);
        m_bestTag.SetActive(chestDesc.m_specialTag == GP_CHEST_TAG.kBest);
        m_popularTag.SetActive(chestDesc.m_specialTag == GP_CHEST_TAG.kPopular);
        m_contentDescText1.text = chestDesc.m_contentDesc1;
        m_contentDescText2.text = chestDesc.m_contentDesc2;
    }

    /// <summary>
    /// Linked to the OnClick event of the buy using gold button component.
    /// Calls the OnClickedBuyUsingGoldEvent to which you can suscribe to handle the buy.
    /// </summary>
    void OnBuyUsingGold()
    {
        if (OnClickedBuyUsingGoldEvent != null)
        {
            OnClickedBuyUsingGoldEvent.Invoke(this);
        }
    }

    /// <summary>
    /// Linked to the OnClick event of the buy using gems button component.
    /// Calls the OnClickedBuyUsingGemsEvent to which you can suscribe to handle the buy.
    /// </summary>
    void OnBuyUsingGems()
    {
        if (OnClickedBuyUsingGemsEvent != null)
        {
            OnClickedBuyUsingGemsEvent.Invoke(this);
        }
    }

    /// <summary>
    /// Called from the store.
    /// </summary>
    public void OnSuccesfullBuy()
    {
        if (OnSuccesfullBuyEvent != null)
        {
            OnSuccesfullBuyEvent.Invoke(this);
        }
    }

}
