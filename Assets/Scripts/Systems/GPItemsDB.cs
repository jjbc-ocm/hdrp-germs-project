using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPItemsDB : MonoBehaviour
{
    [Header("Dummy Parts")]
    [SerializeField]
    List<GPDummyPartDesc> m_dummySkins = new List<GPDummyPartDesc>();
    [SerializeField]
    List<GPDummyPartDesc> m_dummyEyes = new List<GPDummyPartDesc>();
    [SerializeField]
    List<GPDummyPartDesc> m_dummyMouths = new List<GPDummyPartDesc>();
    [SerializeField]
    List<GPDummyPartDesc> m_dummyHairs = new List<GPDummyPartDesc>();
    [SerializeField]
    List<GPDummyPartDesc> m_dummyHorns = new List<GPDummyPartDesc>();
    [SerializeField]
    List<GPDummyPartDesc> m_dummyWears = new List<GPDummyPartDesc>();
    [SerializeField]
    List<GPDummyPartDesc> m_dummyGloves = new List<GPDummyPartDesc>();
    [SerializeField]
    List<GPDummyPartDesc> m_dummyTails = new List<GPDummyPartDesc>();

    public Dictionary<string, GPDummyPartDesc> m_dummyPartsMap = new Dictionary<string, GPDummyPartDesc>();

    [Header("Crews")]
    [SerializeField]
    public List<GPShipDesc> m_crews = new List<GPShipDesc>();
    public Dictionary<string, GPShipDesc> m_crewsMap = new Dictionary<string, GPShipDesc>();

    [Header("Profile icons")]
    [SerializeField]
    public List<GPProfileIconSO> m_profileIcons = new List<GPProfileIconSO>();
    public Dictionary<string, GPProfileIconSO> m_profileIconsMap = new Dictionary<string, GPProfileIconSO>();


    public static GPItemsDB m_instance;

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        for (int i = 0; i < m_dummySkins.Count; i++)
        {
            m_dummyPartsMap.Add(m_dummySkins[i].name, m_dummySkins[i]);
        }

        for (int i = 0; i < m_dummyEyes.Count; i++)
        {
            m_dummyPartsMap.Add(m_dummyEyes[i].name, m_dummyEyes[i]);
        }

        for (int i = 0; i < m_dummyMouths.Count; i++)
        {
            m_dummyPartsMap.Add(m_dummyMouths[i].name, m_dummyMouths[i]);
        }

        for (int i = 0; i < m_dummyHairs.Count; i++)
        {
            m_dummyPartsMap.Add(m_dummyHairs[i].name, m_dummyHairs[i]);
        }

        for (int i = 0; i < m_dummyHorns.Count; i++)
        {
            m_dummyPartsMap.Add(m_dummyHorns[i].name, m_dummyHorns[i]);
        }

        for (int i = 0; i < m_dummyWears.Count; i++)
        {
            m_dummyPartsMap.Add(m_dummyWears[i].name, m_dummyWears[i]);
        }

        for (int i = 0; i < m_dummyGloves.Count; i++)
        {
            m_dummyPartsMap.Add(m_dummyGloves[i].name, m_dummyGloves[i]);
        }

        for (int i = 0; i < m_dummyTails.Count; i++)
        {
            m_dummyPartsMap.Add(m_dummyTails[i].name, m_dummyTails[i]);
        }

        for (int i = 0; i < m_crews.Count; i++)
        {
            m_crewsMap.Add(m_crews[i].name, m_crews[i]);
        }

        for (int i = 0; i < m_profileIcons.Count; i++)
        {
            m_profileIconsMap.Add(m_profileIcons[i].name, m_profileIcons[i]);
        }
    }

    public List<GPDummyPartDesc> GetPartsOfTypeAndRarity(GP_DUMMY_PART_TYPE type, GP_DUMMY_PART_RARITY rarity)
    {
        List<GPDummyPartDesc> parts = new List<GPDummyPartDesc>();
        foreach (KeyValuePair<string, GPDummyPartDesc> entry in m_dummyPartsMap)
        {
            if (entry.Value.m_rarity == rarity && entry.Value.m_type == type)
            {
                parts.Add(entry.Value);
            }
        }
        return parts;
    }

    public List<GPDummyPartDesc> GetPartsOfType(GP_DUMMY_PART_TYPE type)
    {
        List<GPDummyPartDesc> parts = new List<GPDummyPartDesc>();
        foreach (KeyValuePair<string, GPDummyPartDesc> entry in m_dummyPartsMap)
        {
            if (entry.Value.m_type == type)
            {
                parts.Add(entry.Value);
            }
        }
        return parts;
    }

    public List<GPDummyPartDesc> GetPartsOfRarity(GP_DUMMY_PART_RARITY rarity)
    {
        List<GPDummyPartDesc> parts = new List<GPDummyPartDesc>();
        foreach (KeyValuePair<string, GPDummyPartDesc> entry in m_dummyPartsMap)
        {
            if (entry.Value.m_rarity == rarity)
            {
                parts.Add(entry.Value);
            }
        }
        return parts;
    }
}
