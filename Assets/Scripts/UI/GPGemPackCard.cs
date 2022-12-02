using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class GPGemPackCard : MonoBehaviour
{
    public Image m_iconImage;
    public TextMeshProUGUI m_packName;
    public Button m_buyButton;
    public TextMeshProUGUI m_usdPriceText;
    public string m_currencyName = "USD";
    public TextMeshProUGUI m_gemAmountText;
    public GameObject m_bestTag;
    public GameObject m_popularTag;
    public GPStoreGemsSO m_gemPackDesc;

    public UnityEvent<GPStoreGemsSO> OnBuyClickedEvent;

    // Start is called before the first frame update
    void Start()
    {
        m_buyButton.onClick.AddListener(OnBuyClicked);

        if (m_gemPackDesc != null)
        {
            DisplayGemPack(m_gemPackDesc);
        }
    }

    public void DisplayGemPack(GPStoreGemsSO gemPackDesc)
    {
        m_gemPackDesc = gemPackDesc;
        m_iconImage.sprite = gemPackDesc.m_gemIcon;
        m_packName.text = gemPackDesc.m_packName;
        m_usdPriceText.text = gemPackDesc.m_usdPrice.ToString() + " " + m_currencyName;
        m_gemAmountText.text = gemPackDesc.m_gemAmount.ToString();
        m_bestTag.SetActive(gemPackDesc.m_specialTag == GP_GEM_PACK_TAG.kBest);
        m_popularTag.SetActive(gemPackDesc.m_specialTag == GP_GEM_PACK_TAG.kPopular);
        if (gemPackDesc.m_overrideSize)
        {
            m_iconImage.rectTransform.sizeDelta = gemPackDesc.m_sizeOverride;
        }
        if (gemPackDesc.m_overridePosition)
        {
            m_iconImage.rectTransform.localPosition = gemPackDesc.m_posOverride;
        }
    }

    void OnBuyClicked()
    {
        if (OnBuyClickedEvent != null)
        {
            OnBuyClickedEvent.Invoke(m_gemPackDesc);
        }
    }
}
