/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Linq;
using TMPro;

namespace TanksMP
{
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
        private AftermathUI uiAftermath;


        //initialize variables
        void Start()
        {
            //play background music
            AudioManager.Instance.PlayMusic(1);
        }

        void Update()
        {
            /* Update player current gold UI */
            if (Player.Mine != null)
            {
                textPlayerGold.text = Player.Mine.Inventory.Gold.ToString();
            }
        }


        /// <summary>
        /// This method gets called whenever room properties have been changed on the network.
        /// Updating our team size and score UI display during the game.
        /// See the official Photon docs for more details.
        /// </summary>
        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
		{
			//OnTeamSizeChanged(PhotonNetwork.CurrentRoom.GetSize());
			OnTeamScoreChanged(PhotonNetwork.CurrentRoom.GetScore());
		}


        


        /// <summary>
        /// This is an implementation for changes to the team score,
        /// updating the text values (updates UI display of team scores).
        /// </summary>
        public void OnTeamScoreChanged(int[] score)
        {
            //loop over texts
			for(int i = 0; i < score.Length; i++)
            {
                //detect if the score has been increased, then add fancy animation
                //if(score[i] > int.Parse(teamScore[i].text))
                //    teamScore[i].GetComponent<Animator>().Play("Animation");

                //assign score value to text
                teamScore[i].text = score[i].ToString();
            }
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

        public void OpenKillStatement(PhotonView winner, PhotonView loser)
        {
            uiKillStatement.Open((self) =>
            {
                self.Winner = winner;

                self.Loser = loser;
            });
        }

        public void OpenSettings()
        {
            SettingsUI.Instance.Open((self) =>
            {
                self.Data = new SettingsData(APIManager.Instance.PlayerData.Settings);
            });
        }

        public void OpenAftermath(Team team, int winnerTeamIndex)
        {
            uiAftermath.Open((self) =>
            {
                self.WinnerTeam = team;

                self.BattleResult =
                    winnerTeamIndex == -1 ? BattleResultType.Draw :
                    winnerTeamIndex == PhotonNetwork.LocalPlayer.GetTeam() ? BattleResultType.Victory :
                    BattleResultType.Defeat;

                self.IsMessageDone = false;
            });
        }


        /// <summary>
        /// Returns to the starting scene and immediately requests another game session.
        /// In the starting scene we have the loading screen and disconnect handling set up already,
        /// so this saves us additional work of doing the same logic twice in the game scene. The
        /// restart request is implemented in another gameobject that lives throughout scene changes.
        /// </summary>
        public void Restart()
        {
            GameObject gObj = new GameObject("RestartNow");
            gObj.AddComponent<UIRestartButton>();
            DontDestroyOnLoad(gObj);
            
            Disconnect();
        }


        /// <summary>
        /// Stops receiving further network updates by hard disconnecting, then load starting scene.
        /// </summary>
        public void Disconnect()
        {
            SceneManager.LoadScene(Constants.MENU_SCENE_NAME);

            if (PhotonNetwork.IsConnected)
                PhotonNetwork.Disconnect();
        }


        /// <summary>
        /// Loads the starting scene. Disconnecting already happened when presenting the GameOver screen.
        /// </summary>
        public override void OnLeftRoom()
        {
            //SceneManager.LoadScene(Constants.MENU_SCENE_NAME);
            //var reconnectResult = PhotonNetwork.ReconnectAndRejoin();

            //Debug.Log("OnLeftRoom reconnectResult " + reconnectResult);
        }
    }
}