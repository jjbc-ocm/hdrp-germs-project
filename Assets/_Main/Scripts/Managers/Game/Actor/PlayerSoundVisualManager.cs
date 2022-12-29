using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class PlayerSoundVisualManager : GameEntityManager
{
    [SerializeField]
    private GameObject[] teamIndicators;

    [SerializeField]
    private GameObject[] waterIndicators;

    [SerializeField]
    private Sprite spriteIcon;

    [SerializeField]
    private AudioClip shotClip;

    [SerializeField]
    private AudioClip explosionClip;

    [SerializeField]
    private GameObject shotFX;

    [SerializeField]
    private GameObject explosionFX;

    [SerializeField]
    private GameObject rendererAnchor;

    [SerializeField]
    private MeshRenderer rendererShip;

    [SerializeField]
    private GameObject iconIndicator;

    [SerializeField]
    private Material materialNormal;

    [SerializeField]
    private Material materialInvisible;

    [SerializeField]
    private GameObject dummyNormal;

    [SerializeField]
    private GameObject dummyInvisible;

    private PlayerInventoryManager inventory;

    private PlayerStatusManager status;

    private bool isNullifyInvisibilityEffect;

    private bool isInsideBush;

    public Sprite SpriteIcon { get => spriteIcon; }

    public GameObject RendererAnchor { get => rendererAnchor; }

    public MeshRenderer RendererShip { get => rendererShip; }

    public GameObject IconIndicator { get => iconIndicator; }

    void Awake()
    {
        inventory = GetComponent<PlayerInventoryManager>();

        status = GetComponent<PlayerStatusManager>();
    }

    void Start()
    {
        teamIndicators[photonView.GetTeam()].SetActive(true);
    }

    void Update()
    {
        var isInvisible = (inventory.StatModifier.IsInvisible || status.StatModifier.IsInvisible);

        if (photonView.GetTeam() == PhotonNetwork.LocalPlayer.GetTeam())
        {
            rendererShip.material = isInvisible ? materialInvisible : materialNormal;

            dummyNormal.SetActive(!isInvisible);

            dummyInvisible.SetActive(isInvisible);
        }
        else
        {
            if (Player.Mine != null)
            {
                var isInPlayerRange = Vector3.Distance(transform.position, Player.Mine.transform.position) <= Constants.FOG_OF_WAR_DISTANCE;

                var isActive = !isInvisible && (isInPlayerRange || IsInSupremacyWard()) && !isInsideBush;

                rendererShip.gameObject.SetActive(isActive);

                dummyNormal.SetActive(isActive);

                dummyInvisible.SetActive(!dummyNormal.activeSelf);

                teamIndicators[photonView.GetTeam()].SetActive(isActive);

                foreach (var waterIndicator in waterIndicators)
                {
                    waterIndicator.SetActive(isActive);
                }
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        var supremacyWard = col.GetComponent<SupremacyWardEffectManager>();

        if (supremacyWard != null && supremacyWard.Team != photonView.GetTeam())
        {
            isNullifyInvisibilityEffect = true;
        }

        else if (col.gameObject.layer == LayerMask.NameToLayer("Bush"))
        {
            isInsideBush = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        var supremacyWard = col.GetComponent<SupremacyWardEffectManager>();

        if (supremacyWard != null)
        {
            isNullifyInvisibilityEffect = false;
        }

        else if (col.gameObject.layer == LayerMask.NameToLayer("Bush"))
        {
            isInsideBush = false;
        }
    }
}
