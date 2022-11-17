using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GPCrewScreen : GPGUIScreen
{
    [Header("Data")]
    public List<GPShipDesc> m_shipsData;
    List<GPShipCard> m_shipsCards = new List<GPShipCard>();
    public List<Sprite> m_shipTypeImages;
    public Transform m_cardContainer;

    [Header("Prefab references")]
    public GPShipCard m_cardPrefab;

    [Header("Description References")]
    public TextMeshProUGUI m_shipName;
    public TextMeshProUGUI m_shipDesc;
    public TextMeshProUGUI m_shipAbility;

    [Header("Stats References")]
    public TextMeshProUGUI m_health;
    public TextMeshProUGUI m_mana;
    public TextMeshProUGUI m_dmg;
    public TextMeshProUGUI m_abilityPower;
    public TextMeshProUGUI m_atkSpeed;
    public TextMeshProUGUI m_movSpeed;
    public TextMeshProUGUI m_armor;
    public TextMeshProUGUI m_magicResist;

    [Header("Other References")]
    public Transform m_shipModelContainer;

    [Header("Camera Settings")]
    public Camera m_camera;
    public Transform m_homeCameraPos;
    public Transform m_crewCameraPos;
    public float m_transitionTime = 0.7f;

    [HideInInspector]
    public GPShipDesc m_viewedShip = null;
    public GPShipDesc m_selectedShip = null;
    GameObject m_viewedShipModelInstance = null;
    int m_currShipIdx = 0;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(m_shipModelContainer.GetChild(0).gameObject);
        ShowAllShipsTypes();
        m_selectedShip = m_shipsData[0]; // start with default selected.
    }

    public override void Show()
    {
        base.Show();
        // So the description displays the info of the first ship
        OnCurrentShipChanged();

        LeanTween.move(m_camera.gameObject, m_crewCameraPos, m_transitionTime).setEaseSpring();
    }

    public override void Hide()
    {
        base.Hide();

        LeanTween.move(m_camera.gameObject, m_homeCameraPos, m_transitionTime).setEaseSpring();

        ChangeShipModel(m_selectedShip);
    }

    public void ShowAllShipsTypes()
    {
        // Clear old cards
        m_shipsCards.Clear();
        foreach (Transform child in m_cardContainer)
        {
            Destroy(child.gameObject);
        }

        // Spawn new cards of all type
        for (int i = 0; i < m_shipsData.Count; i++)
        {
            GPShipCard card = Instantiate(m_cardPrefab, m_cardContainer);
            card.DisplayShipDesc(m_shipsData[i]);
            card.OnCardClickedEvent.AddListener(ViewShip);
            m_shipsCards.Add(card);
        }

        ViewShip(m_shipsCards[0]);
    }

    public void ShowShipsOfType(int type)
    {
        // Clear old cards
        m_shipsCards.Clear();
        foreach (Transform child in m_cardContainer)
        {
            Destroy(child.gameObject);
        }

        // Spawn new cards of type
        for (int i = 0; i < m_shipsData.Count; i++)
        {
            if ((int)m_shipsData[i].m_type == type)
            {
                GPShipCard card = Instantiate(m_cardPrefab, m_cardContainer);
                card.DisplayShipDesc(m_shipsData[i]);
                card.OnCardClickedEvent.AddListener(ViewShip);
                m_shipsCards.Add(card);
            }
        }

        ViewShip(m_shipsCards[0]);
    }

    public void OnNextShip()
    {
        m_currShipIdx++;

        OnCurrentShipChanged();
    }

    public void OnPrevShip()
    {
        m_currShipIdx--;

        OnCurrentShipChanged();
    }

    public void ViewShip(GPShipCard card)
    {
        for (int i = 0; i < m_shipsCards.Count; i++)
        {
            if (m_shipsCards[i].m_ShipDesc == card.m_ShipDesc)
            {
                m_currShipIdx = i;
                break;
            }
        }
        OnCurrentShipChanged();
    }

    public void OnCurrentShipChanged()
    {
        m_currShipIdx = Mathf.Clamp(m_currShipIdx, 0, m_shipsCards.Count - 1);
        m_viewedShip = m_shipsCards[m_currShipIdx].m_ShipDesc;
        
        m_shipName.text = m_viewedShip.m_name;
        m_shipDesc.text = m_viewedShip.m_desc;
        m_shipAbility.text = m_viewedShip.m_ability;

        m_health.text = m_viewedShip.m_playerPrefab.MaxHealth.ToString();
        m_mana.text = m_viewedShip.m_playerPrefab.MaxMana.ToString();
        m_dmg.text = m_viewedShip.m_playerPrefab.AttackDamage.ToString();
        m_abilityPower.text = m_viewedShip.m_playerPrefab.AbilityPower.ToString();
        m_atkSpeed.text = m_viewedShip.m_playerPrefab.AttackSpeed.ToString();
        m_movSpeed.text = m_viewedShip.m_playerPrefab.MoveSpeed.ToString();
        m_armor.text = m_viewedShip.m_playerPrefab.Armor.ToString();
        m_magicResist.text = m_viewedShip.m_playerPrefab.Armor.ToString();

        ChangeShipModel(m_viewedShip);
    }

    void ChangeShipModel(GPShipDesc desc)
    {
        Destroy(m_viewedShipModelInstance);

        m_viewedShipModelInstance = Instantiate(desc.m_model, m_shipModelContainer);
        m_viewedShipModelInstance.transform.localScale = Vector3.one;
        m_viewedShipModelInstance.transform.localPosition = Vector3.zero;
        m_viewedShipModelInstance.transform.localEulerAngles = Vector3.zero;

        LeanTween.move(m_viewedShipModelInstance, m_viewedShipModelInstance.transform.position + Vector3.down * 10, 0.4f).setEasePunch();
    }

    public void SelectShip()
    {
        m_selectedShip = m_viewedShip;
        LeanTween.scale(m_viewedShipModelInstance, m_viewedShipModelInstance.transform.localScale + (Vector3.one*0.2f), 0.4f).setEasePunch();
    }

}
