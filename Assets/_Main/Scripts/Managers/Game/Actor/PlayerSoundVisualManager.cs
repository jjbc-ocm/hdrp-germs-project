using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class PlayerSoundVisualManager : MonoBehaviour
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

    private PlayerManager player;

    private PlayerInventoryManager inventory;

    private PlayerStatusManager status;

    public Sprite SpriteIcon { get => spriteIcon; }

    public GameObject RendererAnchor { get => rendererAnchor; }

    public MeshRenderer RendererShip { get => rendererShip; }

    public GameObject IconIndicator { get => iconIndicator; }

    #region Unity

    private void Awake()
    {
        player = GetComponent<PlayerManager>();

        inventory = GetComponent<PlayerInventoryManager>();

        status = GetComponent<PlayerStatusManager>();
    }

    private void Start()
    {
        teamIndicators[player.GetTeam()].SetActive(true);
    }

    private void Update()
    {
        var isInvisible = inventory.StatModifier.IsInvisible || status.StatModifier.IsInvisible;

        if (player.GetTeam() == PhotonNetwork.LocalPlayer.GetTeam())
        {
            rendererShip.material = isInvisible ? materialInvisible : materialNormal;

            dummyNormal.SetActive(!isInvisible);

            dummyInvisible.SetActive(isInvisible);
        }
        else
        {
            if (PlayerManager.Mine != null)
            {
                var isActive = !isInvisible && player.IsVisibleRelativeTo(PlayerManager.Mine.GetTeam());

                rendererShip.gameObject.SetActive(isActive);

                dummyNormal.SetActive(isActive);

                dummyInvisible.SetActive(!dummyNormal.activeSelf);

                teamIndicators[player.GetTeam()].SetActive(isActive);

                iconIndicator.SetActive(isActive);

                foreach (var waterIndicator in waterIndicators)
                {
                    waterIndicator.SetActive(isActive);
                }
            }
        }
    }

    #endregion
}
