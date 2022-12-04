using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPTicketScreen : GPGUIScreen
{
    public Transform m_gemPackCardsContainer;
    public GPGemPackCard m_gemPackCardPrefab;
    public List<GPStoreGemsSO> m_gemPacksData;
    List<GPGemPackCard> m_instancedCards = new List<GPGemPackCard>();

    [Header("Audio Settings")]
    public AudioClip m_buySuccedSFX;
    public AudioClip m_buyErrorSFX;

    [Header("Misc.")]

    [SerializeField]
    private GameObject m_LoadIndicator;

    void Start()
    {
        for (int i = 0; i < m_gemPacksData.Count; i++)
        {
            GPGemPackCard newCard = Instantiate(m_gemPackCardPrefab, m_gemPackCardsContainer);
            newCard.DisplayGemPack(m_gemPacksData[i]);
            newCard.OnBuyClickedEvent.AddListener(OnBuyUsingUSD);
            m_instancedCards.Add(newCard);
        }
    }

    public async void OnBuyUsingUSD(GPStoreGemsSO gemPackDesc)
    {
        bool buySucceed = false;
        //TODO: Do API call here and modify buySucceed variable
        //You can get the usd price from the gemPackDesc parameter

        //buySucceed = GPPlayerProfile.m_instance.TrySpendUSD(gemPackDesc.m_usdPrice); // TODO: USD payment
        buySucceed = true;

        if (buySucceed)
        {
            m_LoadIndicator.SetActive(true);
            await APIManager.Instance.PlayerData.AddGems(gemPackDesc.m_gemAmount);
            m_LoadIndicator.SetActive(false);
            TanksMP.AudioManager.Play2D(m_buySuccedSFX);
        }
        else
        {
            TanksMP.AudioManager.Play2D(m_buyErrorSFX);
        }
    }
}
