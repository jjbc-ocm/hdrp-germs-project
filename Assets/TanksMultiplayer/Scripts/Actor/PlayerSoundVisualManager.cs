using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundVisualManager : MonoBehaviourPunCallbacks
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

    private PlayerInventoryManager inventory;

    private PlayerStatusManager status;

    private bool isNullifyInvisibilityEffect;

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
        var isInvisible = (inventory.StatModifier.IsInvisible || status.StatModifier.IsInvisible) && !isNullifyInvisibilityEffect;

        if (photonView.GetTeam() == PhotonNetwork.LocalPlayer.GetTeam())
        {
            rendererShip.material = isInvisible ? materialInvisible : materialNormal;
        }
        else
        {
            rendererShip.gameObject.SetActive(!isInvisible);

            teamIndicators[photonView.GetTeam()].SetActive(!isInvisible);

            foreach (var waterIndicator in waterIndicators)
            {
                waterIndicator.SetActive(!isInvisible);
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
    }

    void OnTriggerExit(Collider col)
    {
        var supremacyWard = col.GetComponent<SupremacyWardEffectManager>();

        if (supremacyWard != null)
        {
            isNullifyInvisibilityEffect = false;
        }
    }
}
