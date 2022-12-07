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

    [Header("Misc.")]

    [SerializeField]
    private GameObject m_LoadIndicator;

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

    public async void OnBuyUsingGold(GPStoreChestCard chestCard)
    {
        //bool buySucceed = false;
        //TODO: Do API call here and modify buySucceed variable
        //You can get the gold price from the chestDesc parameter
        //buySucceed = GPPlayerProfile.m_instance.TrySpendGold(chestCard.m_chestDesc.m_goldPrice);

        m_LoadIndicator.SetActive(true);

        var buySucceed = await APIManager.Instance.TryVirtualPurchase(chestCard.m_chestDesc.GoldID);

        m_LoadIndicator.SetActive(false);

        if (buySucceed)
        {
            chestCard.OnSuccesfullBuy();
            TanksMP.AudioManager.Play2D(m_buySuccedSFX);
            OpenChest(chestCard);
        }
        else
        {
            TanksMP.AudioManager.Play2D(m_buyErrorSFX);
        }
    }

    public async void OnBuyUsingGems(GPStoreChestCard chestCard)
    {
        //bool buySucceed = false;
        //TODO: Do API call here and modify buySucceed variable
        //You can get the gem price from the chestDesc parameter

        //buySucceed = GPPlayerProfile.m_instance.TrySpendGems(chestCard.m_chestDesc.m_gemPrice);

        m_LoadIndicator.SetActive(true);

        var buySucceed = await APIManager.Instance.TryVirtualPurchase(chestCard.m_chestDesc.GemID);

        m_LoadIndicator.SetActive(false);

        if (buySucceed)
        {
            chestCard.OnSuccesfullBuy();
            TanksMP.AudioManager.Play2D(m_buySuccedSFX);
            OpenChest(chestCard);
        }
        else
        {
            TanksMP.AudioManager.Play2D(m_buyErrorSFX);
        }
    }

    public void OpenChest(GPStoreChestCard chestCard)
    {
        chestCard.m_chestDesc.OpenChest();
    }

}
