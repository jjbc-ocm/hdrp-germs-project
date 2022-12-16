using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GPPlayerProfile : MonoBehaviour
{
    /*[Header("Currency settings")]
    public int m_gems;
    public UnityEvent OnGemsModifiedEvent;
    public int m_gold;
    public UnityEvent OnGoldModifiedEvent;*/

    [Header("Energy settings")]
    public int m_energy;
    public int m_maxEnergy = 10;
    public UnityEvent OnEnergyModifiedEvent;

    [Header("Owned items settings")]
    public List<GPStoreChestSO> m_chests;
    public List<GPShipDesc> m_ships;
    public List<GPProfileIconSO> m_profileIcons;
    public List<GPDummyPartDesc> m_dummySkins;
    public List<GPDummyPartDesc> m_dummyEyes;
    public List<GPDummyPartDesc> m_dummyMouths;
    public List<GPDummyPartDesc> m_dummyHairs;
    public List<GPDummyPartDesc> m_dummyHorns;
    public List<GPDummyPartDesc> m_dummyWears;
    public List<GPDummyPartDesc> m_dummyGloves;
    public List<GPDummyPartDesc> m_dummyTails;
    public UnityEvent OnDummyPartsModifiedEvent; // called when player gets new dummy part

    //public List<GPDummyData> m_dummySlots = new List<GPDummyData>();
    //public int m_currDummySlotIdx = 0;

    public static GPPlayerProfile m_instance;

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
        }
    }

    private void Start()
    {
        //TODO: Initialize gold, gems, chests, dummyparts and ships using the data from the API for this player.
    }

    public void AddEnergy(int amount)
    {
        //TODO: maybe we should do an api call here for modifying the amount on the API and then reading back the value.
        m_energy += amount;
        OnEnergyModified();
    }

    /// <summary>
    /// Tries to spend energy if it has enough.
    /// Returns true if user had enough energy to spend, false otherwise.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool TrySpendEnergy(int amount)
    {
        if (m_energy < amount)
        {
            return false;
        }
        //TODO: maybe we should do an api call here for modifying the amount on the API and then reading back the value.
        m_energy -= amount;
        OnEnergyModified();
        return true;
    }

    void OnEnergyModified()
    {
        m_energy = Mathf.Clamp(m_energy, 0, m_maxEnergy);

        if (OnEnergyModifiedEvent != null)
        {
            OnEnergyModifiedEvent.Invoke();
        }
    }

    public void AddDummyPart(GPDummyPartDesc dummyPart)
    {
        switch (dummyPart.m_type)
        {
            case GP_DUMMY_PART_TYPE.kSkin:
                if (!m_dummySkins.Contains(dummyPart))
                {
                    m_dummySkins.Add(dummyPart);
                }
                break;
            case GP_DUMMY_PART_TYPE.kEye:
                if (!m_dummyEyes.Contains(dummyPart))
                {
                    m_dummyEyes.Add(dummyPart);
                }
                break;
            case GP_DUMMY_PART_TYPE.kMouth:
                if (!m_dummyMouths.Contains(dummyPart))
                {
                    m_dummyMouths.Add(dummyPart);
                }
                break;
            case GP_DUMMY_PART_TYPE.kHair:
                if (!m_dummyHairs.Contains(dummyPart))
                {
                    m_dummyHairs.Add(dummyPart);
                }
                break;
            case GP_DUMMY_PART_TYPE.kHorn:
                if (!m_dummyHorns.Contains(dummyPart))
                {
                    m_dummyHorns.Add(dummyPart);
                }
                break;
            case GP_DUMMY_PART_TYPE.kWear:
                if (!m_dummyWears.Contains(dummyPart))
                {
                    m_dummyWears.Add(dummyPart);
                }
                break;
            case GP_DUMMY_PART_TYPE.kGlove:
                if (!m_dummyGloves.Contains(dummyPart))
                {
                    m_dummyGloves.Add(dummyPart);
                }
                break;
            case GP_DUMMY_PART_TYPE.kTail:
                if (!m_dummyTails.Contains(dummyPart))
                {
                    m_dummyTails.Add(dummyPart);
                }
                break;
            default:
                break;
        }

        if (OnDummyPartsModifiedEvent != null)
        {
            OnDummyPartsModifiedEvent.Invoke();
        }
    }

    public void AddShip(GPShipDesc shipDesc)
    {
        if (!m_ships.Contains(shipDesc))
        {
            m_ships.Add(shipDesc);
        }
    }

    public void AddProfileIcon(GPShipDesc shipDesc)
    {
        if (!m_ships.Contains(shipDesc))
        {
            m_ships.Add(shipDesc);
        }
    }

    /*public void AddGold(int amount)
    {
        //TODO: maybe we should do an api call here for modifying the amount on the API and then reading back the value.
        m_gold += amount;
        OnGoldModified();
    }*/

    /// <summary>
    /// Tries to spend gold if it has enough.
    /// Returns true if user had enough gold to spend, false otherwise.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    /*public bool TrySpendGold(int amount)
    {
        if (m_gold < amount)
        {
            return false;
        }
        //TODO: maybe we should do an api call here for modifying the amount on the API and then reading back the value.
        m_gold -= amount;
        OnGoldModified();
        return true;
    }*/

    /*void OnGoldModified()
    {
        m_gold = Mathf.Clamp(m_gold, 0, int.MaxValue);

        if (OnGoldModifiedEvent != null)
        {
            OnGoldModifiedEvent.Invoke();
        }
    }*/

    /*public void AddGems(int amount)
    {
        //TODO: maybe we should do an api call here for modifying the amount on the API and then reading back the value.
        m_gems += amount;
        OnGemsModified();
    }*/

    /// <summary>
    /// Tries to spend gems if it has enough.
    /// Returns true if user had enough gems to spend, false otherwise.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    /*public bool TrySpendGems(int amount)
    {
        //TODO: maybe we should do an api call here for modifying the amount on the API and then reading back the value.
        if (m_gems < amount)
        {
            return false;
        }
        m_gems -= amount;
        OnGemsModified();

        return true;
    }

    void OnGemsModified()
    {
        m_gems = Mathf.Clamp(m_gems, 0, int.MaxValue);

        if (OnGemsModifiedEvent != null)
        {
            OnGemsModifiedEvent.Invoke();
        }
    }*/
}
