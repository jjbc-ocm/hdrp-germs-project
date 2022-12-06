using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum GP_CHEST_TAG
{
    kNone,
    kBest,
    kPopular
}

[CreateAssetMenu(fileName = "GPStoreChestSO", menuName = "ScriptableObjects/GPStoreChestSO")]
public class GPStoreChestSO : ScriptableObject
{
    [SerializeField]
    private string goldId;

    [SerializeField]
    private string gemId;

    public string m_chestName;
    public bool m_canBuyUsingGold = true;
    public int m_goldPrice;
    public bool m_canBuyUsingGems = true;
    public int m_gemPrice;
    public GP_CHEST_TAG m_specialTag;
    public Sprite m_chestIcon;

    [Header("Reward settings")]
    public int m_dummyPartAmount;
    public GP_DUMMY_PART_RARITY m_dummyPartMinRarity;
    public GP_DUMMY_PART_RARITY m_dummyPartMaxRarity;

    public string GoldID { get => goldId; }

    public string GemID { get => gemId; }

    public void OpenChest()
    {
        //Suffle part types so they are randombly picked.
        //We'll pick in order so if whe shuffle the lsit they will be random
        //so if the user gets 3 parts they can be of random types (hair, ayes, mouths, wear, gloves, horns, etc)
        List<GP_DUMMY_PART_TYPE> types = System.Enum.GetValues(typeof(GP_DUMMY_PART_TYPE)).Cast<GP_DUMMY_PART_TYPE>().ToList();
        System.Random rng = new System.Random();
        List<GP_DUMMY_PART_TYPE> randomTypes = types.OrderBy(a => rng.Next()).ToList();

        //Pick random dummy parts
        List<GPDummyPartDesc> dummyRewards = new List<GPDummyPartDesc>();
        for (int i = 0; i < m_dummyPartAmount; i++)
        {
            //Pick parts from minimum random rarity to maximum random rairty
            int randomRarity = Random.Range((int)m_dummyPartMinRarity, (int)m_dummyPartMaxRarity+1);

            //Get list of possible parts that match the random rarity and type.
            var posibleParts = GPItemsDB.m_instance.GetPartsOfTypeAndRarity(randomTypes[i], (GP_DUMMY_PART_RARITY)randomRarity);

            //Pick random part from the posible parts.
            int randomPartIdx = Random.Range(0, posibleParts.Count);
            dummyRewards.Add(posibleParts[randomPartIdx]);
        }

        //TODO: Add rewards to player

    }
}
