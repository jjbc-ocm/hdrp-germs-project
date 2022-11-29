using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPTicketScreen : GPGUIScreen
{
    public Transform m_gemPackCardsContainer;
    public GPGemPackCard m_gemPackCardPrefab;
    public List<GPStoreGemsSO> m_gemPacksData;
    List<GPGemPackCard> m_instancedCards = new List<GPGemPackCard>();

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

    public void OnBuyUsingUSD(GPStoreGemsSO gemPackDesc)
    {
        //Do API call here
        //You can get the usd price from the gemPackDesc parameter
        Debug.Log("Buy " + gemPackDesc.m_packName + gemPackDesc.m_usdPrice.ToString());
    }
}
