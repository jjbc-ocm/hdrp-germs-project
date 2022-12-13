using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Linq;

public class GPWaitingRoom : MonoBehaviourPunCallbacks, IPunObservable
{
    [System.Serializable]
    public class GPPlayerPanel
    {
        public Image m_shipImage;
        public TextMeshProUGUI m_userNameText;
        public Transform m_movableHolder;
        public string m_assignedUserID = "";
    }

    public PhotonView m_photonView;

    public int m_maxTeamPlayerCount = 3;
    public Button m_teamBlueButton;
    public Button m_teamRedButton;

    public List<GPPlayerPanel> m_blueTeamPanels;
    public List<GPPlayerPanel> m_redTeamPanels;
    public float m_blueTeamReadyXOffset = -50.0f;
    public float m_redTeamReadyXOffset = 50.0f;
    public float m_slideAnimationDuration = 0.5f;

    public GPGUIScreen m_preWaitingScreen;
    public GPGUIScreen m_waitingScreen;

    public Transform m_cardContainer;
    public GPShipCard m_cardPrefab;
    List<GPShipCard> m_shipsCards = new List<GPShipCard>();
    private GPShipDesc m_selectedShip;
    private int m_currShipIdx;
    private GPShipDesc m_viewedShip;
    private GPShipCard m_prevViewedCard;

    [Header("Audio Settings")]
    public AudioClip m_shipChangedSFX;
    public AudioClip m_shipSelectedSFX;

    [Header("Misc.")]

    [SerializeField]
    private GameObject m_LoadIndicator;

    public GPShipDesc SelectedShip { get => m_selectedShip; }

    [Header("Timer")]
    public List<TextMeshProUGUI> m_timersText;
    float m_readyWaitCountDown = 60.0f;
    float m_readyWaitStartTime = 0.0f;
    bool m_levelLoadedCalled = false;


    [Header("Ready")]
    List<string> m_playersReady = new List<string>();
    bool m_weighAnchorAlreadyPressed = false;


    [Header("Other References")]
    public GameObject m_crewSelectionWindow;


    // Start is called before the first frame update
    void Start()
    {
        ClearPanels();

        m_preWaitingScreen.Show();
        m_waitingScreen.Hide();

        m_selectedShip = GPPlayerProfile.m_instance.m_ships.FirstOrDefault(i => i.ID == APIManager.Instance.PlayerData.SelectedShipID);

        // Spawn new cards of all type
        for (int i = 0; i < GPPlayerProfile.m_instance.m_ships.Count; i++)
        {
            GPShipCard card = Instantiate(m_cardPrefab, m_cardContainer);
            card.DisplayShipDesc(GPPlayerProfile.m_instance.m_ships[i]);
            card.OnCardClickedEvent.AddListener(ViewShip);
            m_shipsCards.Add(card);
        }

        m_currShipIdx = 0;
        ViewShip(m_shipsCards[0]);

        m_readyWaitStartTime = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(PhotonNetwork.NetworkClientState);
        if (PhotonNetwork.InRoom)
        {
            m_teamBlueButton.interactable = GetNumberOfPlayersInTeam(0) < m_maxTeamPlayerCount;
            m_teamBlueButton.interactable = GetNumberOfPlayersInTeam(1) < m_maxTeamPlayerCount;

            if (PhotonNetwork.IsMasterClient)
            {
                m_readyWaitCountDown = 30 - (Time.realtimeSinceStartup - m_readyWaitStartTime);
                if (m_readyWaitCountDown < 0)
                {
                    m_readyWaitCountDown = 0; // just so UI doesn't show negative numbers

                    if (!m_levelLoadedCalled)
                    {
                        m_levelLoadedCalled = true;
                        PhotonNetwork.LoadLevel(Constants.GAME_SCENE_NAME);
                    }
                }
            }

            foreach (var timer in m_timersText)
            {
                var ts = TimeSpan.FromSeconds(m_readyWaitCountDown);
                timer.text = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
            }
        }
        else
        {

        }
       
    }

    public void JoinGamePressed()
    {
        GPWaitingRoomNetworkManager.Instance.JoinRoom();
        PhotonNetwork.JoinRandomRoom();
    }

    public void ChooseTeam(int teamIdx)
    {
        //count the players in taht team
        int playersInSelectedTeam = GetNumberOfPlayersInTeam(teamIdx);

        //check if player can still join the team.
        if (playersInSelectedTeam < m_maxTeamPlayerCount)
        {
            //join the team
            PhotonNetwork.LocalPlayer.SetTeam(teamIdx);
        }
        else
        {
            //play error sound probably
        }

        m_preWaitingScreen.Hide();
        m_waitingScreen.Show();

    }

    int GetNumberOfPlayersInTeam(int teamIdx)
    {
        int playersInTeam = 0;
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.GetTeam() == teamIdx)
            {
                playersInTeam++;
            }
        }
        return playersInTeam;
    }

    public void OnWeighAnchorButtonPressed()
    {
        if (m_weighAnchorAlreadyPressed)
        {
            return;
        }
        m_weighAnchorAlreadyPressed = true;
        m_crewSelectionWindow.SetActive(false);
        m_photonView.RPC("RPCSetReady", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.UserId);
    }

    /// <summary>
    /// View a clicked ship
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
        SelectShip();
    }

    /// <summary>
    /// Things that need to be updated when the previewed ship cahnges.
    /// </summary>
    void OnCurrentShipChanged()
    {
        m_currShipIdx = Mathf.Clamp(m_currShipIdx, 0, m_shipsCards.Count - 1);
        m_viewedShip = m_shipsCards[m_currShipIdx].m_ShipDesc;

        if (m_prevViewedCard)
        {
            LeanTween.scale(m_prevViewedCard.gameObject, m_cardPrefab.transform.localScale, 0.3f).setEaseSpring();
        }
        LeanTween.scale(m_shipsCards[m_currShipIdx].gameObject, m_cardPrefab.transform.localScale * 1.05f, 0.3f).setEaseSpring();
    }

    /// <summary>
    /// Confirm ship to use.
    /// Will use the currently previewd one.
    /// </summary>
    public async void SelectShip()
    {
        m_LoadIndicator.SetActive(true);

        m_selectedShip = m_viewedShip;

        APIManager.Instance.PlayerData.SetSelectedShipID(m_selectedShip.ID);
        PhotonNetwork.LocalPlayer.SetShipIdx(SelectedShip.m_prefabListIndex);

        await APIManager.Instance.PlayerData.Put();

        m_LoadIndicator.SetActive(false);

        TanksMP.AudioManager.Play2D(m_shipSelectedSFX);

        m_photonView.RPC("UpdatePlayerPanelsUI", RpcTarget.All);
    }

    void ClearPanels()
    {
        //Clear old data.
        for (int i = 0; i < m_blueTeamPanels.Count; i++)
        {
            m_blueTeamPanels[i].m_userNameText.text = "";
            m_blueTeamPanels[i].m_assignedUserID = "";
            m_blueTeamPanels[i].m_shipImage.enabled = false;
        }
        for (int i = 0; i < m_redTeamPanels.Count; i++)
        {
            m_redTeamPanels[i].m_userNameText.text = "";
            m_redTeamPanels[i].m_assignedUserID = "";
            m_redTeamPanels[i].m_shipImage.enabled = false;
        }
    }

    [PunRPC]
    public void UpdatePlayerPanelsUI()
    {
        //Clear old data.
        ClearPanels();

        //Update with new data.
        int blueidx = 0;
        int redidx = 0;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i].GetTeam() == 0)
            {
                m_blueTeamPanels[blueidx].m_userNameText.text = PhotonNetwork.PlayerList[i].NickName;
                m_blueTeamPanels[blueidx].m_assignedUserID = PhotonNetwork.PlayerList[i].UserId;
                m_blueTeamPanels[blueidx].m_shipImage.enabled = true;
                m_blueTeamPanels[blueidx].m_shipImage.sprite = GPItemsDB.m_instance.m_crews[PhotonNetwork.PlayerList[i].GetShipIndex()].m_shipIconImage;
                blueidx++;
            }
            else if (PhotonNetwork.PlayerList[i].GetTeam() == 1)
            {
                m_redTeamPanels[redidx].m_userNameText.text = PhotonNetwork.PlayerList[i].NickName;
                m_redTeamPanels[redidx].m_assignedUserID = PhotonNetwork.PlayerList[i].UserId;
                m_redTeamPanels[redidx].m_shipImage.enabled = true;
                m_redTeamPanels[redidx].m_shipImage.sprite = GPItemsDB.m_instance.m_crews[PhotonNetwork.PlayerList[i].GetShipIndex()].m_shipIconImage;
                redidx++;
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (PhotonNetwork.NetworkClientState == ClientState.Leaving)
        {
            return;
        }

        if (stream.IsWriting)
        {
            stream.SendNext(this.m_readyWaitCountDown);
            stream.SendNext(this.m_readyWaitStartTime);
        }
        else
        {
            this.m_readyWaitCountDown = (float)stream.ReceiveNext();
            this.m_readyWaitStartTime = (float)stream.ReceiveNext();
        }
    }

    public int GetNumberOfPlayersReady()
    {
        int readyCount = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (m_playersReady.Contains(player.UserId))
            {
                readyCount++;
            }
        }
        return readyCount;
    }

    [PunRPC]
    public void RPCSetReady(string UserId)
    {
        if (!m_playersReady.Contains(UserId))
        {
            m_playersReady.Add(UserId);
        }

        foreach (var panels in m_blueTeamPanels)
        {
            if (panels.m_assignedUserID == UserId)
            {
                LeanTween.moveX(panels.m_movableHolder.gameObject, panels.m_movableHolder.position.x + m_blueTeamReadyXOffset, m_slideAnimationDuration).setEaseSpring();
            }
        }

        foreach (var panels in m_redTeamPanels)
        {
            if (panels.m_assignedUserID == UserId)
            {
                LeanTween.moveX(panels.m_movableHolder.gameObject, panels.m_movableHolder.position.x + m_redTeamReadyXOffset, m_slideAnimationDuration).setEaseSpring();
            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        if (changedProps.ContainsKey(PlayerExtension.team) || changedProps.ContainsKey(PlayerExtension.shipIndex))
        {
            m_photonView.RPC("UpdatePlayerPanelsUI", RpcTarget.All);
        }
    }
}
