using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using System.Linq;

public class GPCrewScreen : GPGUIScreen
{
    public static GPCrewScreen Instance;

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

    [Header("Tab Settings")]
    public Transform m_tabFocusImage;

    private GPShipDesc m_viewedShip;

    private GPShipDesc m_selectedShip;

    private GameObject m_viewedShipModelInstance;

    private int m_currShipIdx;

    private GPShipCard m_prevViewedCard;

    [Header("Audio Settings")]
    public AudioClip m_shipChangedSFX;
    public AudioClip m_shipSelectedSFX;

    [Header("Misc.")]
    
    [SerializeField]
    private GameObject m_LoadIndicator;

    public GPShipDesc SelectedShip { get => m_selectedShip; }
    
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Destroy(m_shipModelContainer.GetChild(0).gameObject);
        ShowAllShipsTypes();

        m_selectedShip = m_shipsData.FirstOrDefault(i => i.ID == APIManager.Instance.PlayerData.SelectedShipID);

        ChangeShipModel(m_selectedShip);
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

    /// <summary>
    /// Displays on the ship list all ships of all kinds
    /// </summary>
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

        m_currShipIdx = 0;
        ViewShip(m_shipsCards[0]);
    }

    /// <summary>
    /// Displays on the ship list only the ships of a certain type
    /// </summary>
    /// <param name="type"></param>
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

        m_currShipIdx = 0;
        ViewShip(m_shipsCards[0]);
    }

    /// <summary>
    /// View next ship model and stats.
    /// </summary>
    public void OnNextShip()
    {
        m_prevViewedCard = m_shipsCards[m_currShipIdx];

        m_currShipIdx++;

        OnCurrentShipChanged();
    }


    /// <summary>
    /// View previus ship model and stats.
    /// </summary>
    public void OnPrevShip()
    {
        m_prevViewedCard = m_shipsCards[m_currShipIdx];

        m_currShipIdx--;

        OnCurrentShipChanged();
    }

    /// <summary>
    /// View a clicked ship model and stats.
    /// </summary>
    /// <param name="card"></param>
    public void ViewShip(GPShipCard card)
    {
        m_prevViewedCard = m_shipsCards[m_currShipIdx];

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

    /// <summary>
    /// Things that need to be updated when the previewed ship cahnges.
    /// </summary>
    void OnCurrentShipChanged()
    {
        if (m_visible)
        {
            AudioManager.Instance.Play2D(m_shipChangedSFX);
        }

        m_currShipIdx = Mathf.Clamp(m_currShipIdx, 0, m_shipsCards.Count - 1);
        m_viewedShip = m_shipsCards[m_currShipIdx].m_ShipDesc;
        
        m_shipName.text = m_viewedShip.m_name;
        m_shipDesc.text = m_viewedShip.m_desc;
        m_shipAbility.text = m_viewedShip.m_ability;

        m_health.text = m_viewedShip.m_playerPrefab.Stat.MaxHealth().ToString();
        m_mana.text = m_viewedShip.m_playerPrefab.Stat.MaxMana().ToString();
        m_dmg.text = m_viewedShip.m_playerPrefab.Stat.AttackDamage().ToString();
        m_abilityPower.text = m_viewedShip.m_playerPrefab.Stat.AbilityPower().ToString();
        m_atkSpeed.text = m_viewedShip.m_playerPrefab.Stat.AttackSpeed().ToString();
        m_movSpeed.text = m_viewedShip.m_playerPrefab.Stat.MoveSpeed().ToString();
        m_armor.text = m_viewedShip.m_playerPrefab.Stat.Armor().ToString();
        m_magicResist.text = m_viewedShip.m_playerPrefab.Stat.Resist().ToString();

        if (m_prevViewedCard)
        {
            LeanTween.scale(m_prevViewedCard.gameObject, m_cardPrefab.transform.localScale, 0.3f).setEaseSpring();
        }
        LeanTween.scale(m_shipsCards[m_currShipIdx].gameObject, m_cardPrefab.transform.localScale * 1.05f, 0.3f).setEaseSpring();

        ChangeShipModel(m_viewedShip);
    }

    /// <summary>
    /// Changes the preview ship model with the one of the scritpable object given as parameter.
    /// </summary>
    /// <param name="desc"></param>
    void ChangeShipModel(GPShipDesc desc)
    {
        string oldName = "";
        if (m_viewedShipModelInstance)
        {
            oldName = m_viewedShipModelInstance.name;
        }
        Destroy(m_viewedShipModelInstance);

        m_viewedShipModelInstance = Instantiate(desc.m_model, m_shipModelContainer);
        m_viewedShipModelInstance.transform.localScale = Vector3.one;
        m_viewedShipModelInstance.transform.localPosition = Vector3.zero;
        m_viewedShipModelInstance.transform.localEulerAngles = Vector3.zero;

        if (m_visible)
        {
            if (oldName == m_viewedShipModelInstance.name) // if old viewed ship is the same ias teh new one then play another animation (end of list)
            {
                LeanTween.move(m_viewedShipModelInstance, m_viewedShipModelInstance.transform.position + Vector3.right * 10, 0.4f).setEasePunch();
            }
            else
            {
                LeanTween.move(m_viewedShipModelInstance, m_viewedShipModelInstance.transform.position + Vector3.down * 10, 0.4f).setEasePunch();
            }
        }
    }

    /// <summary>
    /// Confirm ship to use.
    /// Will use the currently previewd one.
    /// </summary>
    public async void SelectShip()
    {
        m_LoadIndicator.SetActive(true);

        m_selectedShip = m_viewedShip;
        LeanTween.scale(m_viewedShipModelInstance, m_viewedShipModelInstance.transform.localScale - (Vector3.one*0.2f), 0.4f).setEasePunch();
        //PhotonNetwork.LocalPlayer.SetSelectedShipIdx(m_selectedShip.m_prefabListIndex);

        APIManager.Instance.PlayerData.SetSelectedShipID(m_selectedShip.ID);

        await APIManager.Instance.PlayerData.Put();

        m_LoadIndicator.SetActive(false);

        AudioManager.Instance.Play2D(m_shipSelectedSFX);
    }

    /// <summary>
    /// Animates selection tab sprite.
    /// </summary>
    /// <param name="targetTransform"></param>
    public void MoveTapFocus(Transform targetTransform)
    {
        LeanTween.move(m_tabFocusImage.gameObject, targetTransform.position, 0.3f).setEaseSpring();
    }

}
