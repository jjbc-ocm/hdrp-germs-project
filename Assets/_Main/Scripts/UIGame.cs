
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Linq;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// UI script for all elements, team events and user interactions in the game scene.
/// </summary>
public class UIGame : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private TMP_Text[] teamScore;

    [SerializeField]
    private TMP_Text textPlayerGold;

    [SerializeField]
    private TMP_Text spawnDelayText;

    [SerializeField]
    private KillStatementUI uiKillStatement;

    [SerializeField]
    private MessageBroadcastUI uiMessageBroadcast;

    [SerializeField]
    private GuideUI uiGuide;

    [Header("Sound and Visuals")]

    [SerializeField]
    private AudioClip soundAddGuide;

    [SerializeField]
    private AudioClip soundRemoveGuide;

    private float m_currDisplayedGold = 0;

    [SerializeField]
    private float m_displayedGoldAnimSpeed = 5.0f;

    void Update()
    {
        /* Update player current gold UI */
        if (PlayerManager.Mine != null)
        {
            m_currDisplayedGold = Mathf.Lerp(m_currDisplayedGold, PlayerManager.Mine.Inventory.Gold, m_displayedGoldAnimSpeed * Time.deltaTime);
            textPlayerGold.text = Mathf.RoundToInt(m_currDisplayedGold).ToString(); // TODO: how about put it in player info UI instead?
        }
    }


    /// <summary>
    /// This method gets called whenever room properties have been changed on the network.
    /// Updating our team size and score UI display during the game.
    /// See the official Photon docs for more details.
    /// </summary>
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        OnTeamScoreChanged(PhotonNetwork.CurrentRoom.GetScore(0), PhotonNetwork.CurrentRoom.GetScore(1));
    }





    /// <summary>
    /// This is an implementation for changes to the team score,
    /// updating the text values (updates UI display of team scores).
    /// </summary>
    public void OnTeamScoreChanged(int score0, int score1)
    {
        teamScore[0].text = score0.ToString();
        teamScore[1].text = score1.ToString();
    }



    /// <summary>
    /// Set respawn delay value displayed to the absolute time value received.
    /// The remaining time value is calculated in a coroutine by GameManager.
    /// </summary>
    public void SetSpawnDelay(float time)
    {
        spawnDelayText.text = Mathf.Ceil(time) + "";
    }


    /// <summary>
    /// Hides any UI components related to player death after respawn.
    /// </summary>
    public void DisableDeath()
    {
        //clear text component values
        //deathText.text = string.Empty;
        spawnDelayText.text = string.Empty;
    }

    /*public void OpenKillStatement(ActorManager winner, ActorManager loser)
    {
        uiKillStatement.Open((self) =>
        {
            self.Winner = winner;

            self.Loser = loser;
        });
    }*/

    public void OpenSettings()
    {
        SettingsUI.Instance.Open((self) =>
        {
            self.Data = new SettingsData(APIManager.Instance.PlayerData.Settings);
        });
    }

    public void AddGuideItem(GuideData data)
    {
        uiGuide.RefreshUI((self) =>
        {
            if (self.Data == null)
            {
                self.Data = new List<GuideData>();
            }

            if (!self.Data.Contains(data))
            {
                self.Data.Add(data);
            }
        });

        AudioManager.Instance.Play2D(soundAddGuide);
    }

    public void RemoveGuideItem(GuideData data)
    {
        uiGuide.RefreshUI((self) =>
        {
            self.Data.RemoveAll((i) => i == data);
        });

        AudioManager.Instance.Play2D(soundRemoveGuide);
    }




    /// <summary>
    /// Stops receiving further network updates by hard disconnecting, then load starting scene.
    /// </summary>
    public void Disconnect()
    {
        SceneManager.LoadScene(SOManager.Instance.Constants.SceneMenu);

        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();
    }





    #region Photon

    [PunRPC]
    public void RpcKillStatement(int winnerId, int loserId)
    {
        var winner = PhotonView.Find(winnerId).GetComponent<ActorManager>();

        var loser = PhotonView.Find(loserId).GetComponent<ActorManager>();

        uiKillStatement.Open((self) =>
        {
            self.Winner = winner;

            self.Loser = loser;
        });
    }

    [PunRPC]
    public void RpcBroadcastMessage(int playerId, MessageBroadcastType messageBroadcast)
    {
        var player = PhotonView.Find(playerId).GetComponent<ActorManager>();

        uiMessageBroadcast.Open((self) =>
        {
            self.Player = player;

            self.Type = messageBroadcast;
        });
    }

    #endregion
}