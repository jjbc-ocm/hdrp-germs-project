using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GPPlayerProfile : MonoBehaviour
{
    [Header("Energy settings")]
    public int m_energy;
    public int m_maxEnergy = 10;
    public UnityEvent OnEnergyModifiedEvent;

    [Header("Owned items settings")]
    public List<GPShipDesc> m_ships;
    public List<GPProfileIconSO> m_profileIcons;
    public List<DummyPartSO> m_dummySkins;
    public List<DummyPartSO> m_dummyEyes;
    public List<DummyPartSO> m_dummyMouths;
    public List<DummyPartSO> m_dummyHairs;
    public List<DummyPartSO> m_dummyHorns;
    public List<DummyPartSO> m_dummyWears;
    public List<DummyPartSO> m_dummyGloves;
    public List<DummyPartSO> m_dummyTails;
    public UnityEvent OnDummyPartsModifiedEvent; // called when player gets new dummy part

    [Header("Social Settings")]
    public List<GPFriend> m_friends = new List<GPFriend>(); // still not sure how the API will manage this but i'll use this data in the meantime for the UI building.

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

    /// <summary>
    /// Called whenever the energy is modified.
    /// Invokes OnEnergyModifiedEvent.
    /// </summary>
    void OnEnergyModified()
    {
        m_energy = Mathf.Clamp(m_energy, 0, m_maxEnergy);

        if (OnEnergyModifiedEvent != null)
        {
            OnEnergyModifiedEvent.Invoke();
        }
    }

    /// <summary>
    /// Adds a dummy part to the owned parts by the user locally.
    /// </summary>
    /// <param name="dummyPart"></param>
    public void AddDummyPart(DummyPartSO dummyPart)
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
           /* case GP_DUMMY_PART_TYPE.kHair:
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
                break;*/
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

    /// <summary>
    /// Adds a ship to the owned ships by the user locally.
    /// </summary>
    /// <param name="shipDesc"></param>
    public void AddShip(GPShipDesc shipDesc)
    {
        if (!m_ships.Contains(shipDesc))
        {
            m_ships.Add(shipDesc);
        }
    }

    /// <summary>
    /// Adds a profile icon to the owned parts by the user locally.
    /// </summary>
    /// <param name="iconDesc"></param>
    public void AddProfileIcon(GPProfileIconSO iconDesc)
    {
        if (!m_profileIcons.Contains(iconDesc))
        {
            m_profileIcons.Add(iconDesc);
        }
    }

}
