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
        private Image[] team1PlayerIndicators;

        [SerializeField]
        private Image[] team2PlayerIndicators;

        [SerializeField]
        private Image[] team1ChestIndicators;

        [SerializeField]
        private Image[] team2ChestIndicators;

        [SerializeField]
        private ShipHUD[] team1ShipHuds;

        [SerializeField]
        private ShipHUD[] team2ShipHuds;

        [SerializeField]
        private TMP_Text[] teamScore;

        [SerializeField]
        private TMP_Text textPlayerGold;

        [SerializeField]
        private TMP_Text deathText;

        [SerializeField]
        private TMP_Text spawnDelayText;

        [SerializeField]
        private TMP_Text gameOverText;

        [SerializeField]
        private GameObject gameOverMenu;

        public GameObject GameOverMenu { get => gameOverMenu; }


        //initialize variables
        void Start()
        {
            //on non-mobile devices hide joystick controls, except in editor
            /*#if !UNITY_EDITOR && (UNITY_STANDALONE || UNITY_WEBGL)
                ToggleControls(false);
            #endif*/
            
            //on mobile devices enable additional aiming indicator
            /*#if !UNITY_EDITOR && !UNITY_STANDALONE && !UNITY_WEBGL
            if (aimIndicator != null)
            {
                Transform indicator = Instantiate(aimIndicator).transform;
                indicator.SetParent(GameManager.GetInstance().localPlayer.shotPos);
                indicator.localPosition = new Vector3(0f, 0f, 3f);
            }
            #endif*/

            //play background music
            AudioManager.PlayMusic(1);
        }

        void Update()
        {
            var players = FindObjectsOfType<Player>();

            var team1 = players.Where(i => i.photonView.GetTeam() == 0).ToArray();

            var team2 = players.Where(i => i.photonView.GetTeam() == 1).ToArray();

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < (i == 0 ? team1.Count() : team2.Count()); j++)
                {
                    if (i == 0)
                    {
                        team1ShipHuds[j].Player = j < team1.Count() ? team1[j] : null;
                    }

                    if (i == 1)
                    {
                        team2ShipHuds[j].Player = j < team2.Count() ? team2[j] : null;
                    }
                }
            }
            var gold = PhotonNetwork.LocalPlayer.GetGold();
            textPlayerGold.text = gold.ToString();
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
        /// This is an implementation for changes to the team fill,
        /// updating the slider values (updates UI display of team fill).
        /// </summary>
        public void OnTeamSizeChanged(int[] size)
        {
            

            for (int i = 0; i < 3; i++)
            {
                team1PlayerIndicators[i].gameObject.SetActive(false);

                team2PlayerIndicators[i].gameObject.SetActive(false);

                team1ShipHuds[i].Player = null;

                team2ShipHuds[i].Player = null;
            }

            //loop over sliders values and assign it
			for(int i = 0; i < size.Length; i++)
            {
                for (int j = 0; j < size[i]; j++)
                {
                    if (i == 0)
                    {
                        team1PlayerIndicators[j].gameObject.SetActive(true);
                    }

                    if (i == 1)
                    {
                        team2PlayerIndicators[j].gameObject.SetActive(true);
                    }
                }
            }
        }

        /// <summary>
        /// Added by: Jilmer John Cariaso
        /// </summary>
        public void OnChestPickup(Player obj)
        {
            var players = FindObjectsOfType<Player>();

            var team1 = players.Where(i => i.photonView.GetTeam() == 0).ToArray();

            var team2 = players.Where(i => i.photonView.GetTeam() == 1).ToArray();

            for (int i = 0; i < team1ChestIndicators.Length; i++)
            {
                team1ChestIndicators[i].gameObject.SetActive(obj != null && team1.Count() > i && team1[i] == obj);
            }

            for (int i = 0; i < team2ChestIndicators.Length; i++)
            {
                team2ChestIndicators[i].gameObject.SetActive(obj != null && team2.Count() > i && team2[i] == obj);
            }
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
                if(score[i] > int.Parse(teamScore[i].text))
                    teamScore[i].GetComponent<Animator>().Play("Animation");

                //assign score value to text
                teamScore[i].text = score[i].ToString();
            }
        }


        /// <summary>
        /// Enables or disables visibility of joystick controls.
        /// </summary>
        /*public void ToggleControls(bool state)
        {
            for (int i = 0; i < controls.Length; i++)
                controls[i].gameObject.SetActive(state);
        }*/


        /// <summary>
        /// Sets death text showing who killed the player in its team color.
        /// Parameters: killer's name, killer's team
        /// </summary>
        public void SetDeathText(string playerName, Team team)
        {
            //hide joystick controls while displaying death text
            //#if UNITY_EDITOR || (!UNITY_STANDALONE && !UNITY_WEBGL)
            //ToggleControls(false);
            //#endif

            Debug.Log((deathText == null) + " " + (playerName == null) + " " + (team == null));
            
            //show killer name and colorize the name converting its team color to an HTML RGB hex value for UI markup
            deathText.text = "KILLED BY\n<color=#" + ColorUtility.ToHtmlStringRGB(team.material.color) + ">" + playerName + "</color>";
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
            //show joystick controls after disabling death text
            #if UNITY_EDITOR || (!UNITY_STANDALONE && !UNITY_WEBGL)
                //ToggleControls(true);
            #endif
            
            //clear text component values
            deathText.text = string.Empty;
            spawnDelayText.text = string.Empty;
        }


        /// <summary>
        /// Set game end text and display winning team in its team color.
        /// </summary>
        public void SetGameOverText(Team team)
        {
            //hide joystick controls while displaying game end text
            #if UNITY_EDITOR || (!UNITY_STANDALONE && !UNITY_WEBGL)
                //ToggleControls(false);
            #endif
            
            //show winning team and colorize it by converting the team color to an HTML RGB hex value for UI markup
            gameOverText.text = "TEAM <color=#" + ColorUtility.ToHtmlStringRGB(team.material.color) + ">" + team.name + "</color> WINS!";
        }


        /// <summary>
        /// Displays the game's end screen. Called by GameManager after few seconds delay.
        /// Tries to display a video ad, if not shown already.
        /// </summary>
        public void ShowGameOver()
        {       
            //hide text but enable game over window
            gameOverText.gameObject.SetActive(false);
            gameOverMenu.SetActive(true);
            
            //check whether an ad was shown during the game
            //if no ad was shown during the whole round, we request one here
            #if UNITY_ADS
            if(!UnityAdsManager.didShowAd())
                UnityAdsManager.ShowAd(true);
            #endif
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
            if (PhotonNetwork.IsConnected)
                PhotonNetwork.Disconnect();
        }


        /// <summary>
        /// Loads the starting scene. Disconnecting already happened when presenting the GameOver screen.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(NetworkManagerCustom.GetInstance().offlineSceneIndex);
        }
    }
}
