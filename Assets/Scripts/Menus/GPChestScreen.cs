using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPChestScreen : GPGUIScreen
{
    public Transform m_chestCardsContainer;
    public GPStoreChestCard m_chestCardPrefab;
    public List<GPStoreChestSO> m_chestsData;
    List<GPStoreChestCard> m_instancedCards = new List<GPStoreChestCard>();

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
        //Do API call here
        //You can get the gold price from the chestDesc parameter
    }

    public void OnBuyUsingGems(GPStoreChestSO chestDesc)
    {
        //Do API call here
        //You can get the gem price from the chestDesc parameter
    }

}
