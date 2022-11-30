using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPChestScreen : GPGUIScreen
{
    public Transform m_chestCardsContainer;
    public GPStoreChestCard m_chestCardPrefab;
    public List<GPStoreChestSO> m_chestsData;
    List<GPStoreChestCard> m_instancedCards = new List<GPStoreChestCard>();

    [Header("Audio Settings")]
    public AudioClip m_buySuccedSFX;
    public AudioClip m_buyErrorSFX;

    void Start()
    {
        for (int i = 0; i < m_chestsData.Count; i++)
        {
            GPStoreChestCard newCard = Instantiate(m_chestCardPrefab, m_chestCardsContainer);
            newCard.DisplayChest(m_chestsData[i]);
            newCard.OnClickedBuyUsingGoldEvent.AddListener(OnBuyUsingGold);
            newCard.OnClickedBuyUsingGemsEvent.AddListener(OnBuyUsingGems);
            m_instancedCards.Add(newCard);
        }
    }

    public void OnBuyUsingGold(GPStoreChestSO chestDesc)
    {
        bool buySucceed = true;
        //TODO: Do API call here and modify buySucceed variable
        //You can get the gold price from the chestDesc parameter
        if (buySucceed)
        {
            TanksMP.AudioManager.Play2D(m_buySuccedSFX);
        }
        else
        {
            TanksMP.AudioManager.Play2D(m_buyErrorSFX);
        }
    }

    public void OnBuyUsingGems(GPStoreChestSO chestDesc)
    {
        bool buySucceed = true;
        //TODO: Do API call here and modify buySucceed variable
        //You can get the gem price from the chestDesc parameter
        if (buySucceed)
        {
            TanksMP.AudioManager.Play2D(m_buySuccedSFX);
        }
        else
        {
            TanksMP.AudioManager.Play2D(m_buyErrorSFX);
        }
    }

}
