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

        [Header("Self HUD")]
        [SerializeField]
        private Slider sliderHealth;

        [SerializeField]
        private Slider sliderMana;

        [SerializeField]
        private TMP_Text textHealth;

        [SerializeField]
        private TMP_Text textMana;

        public GameObject GameOverMenu { get => gameOverMenu; }


        //initialize variables
        void Start()
        {
            //play background music
            AudioManager.PlayMusic(1);
        }

        void Update()
        {
            var players = FindObjectsOfType<Player>();

            var myPlayer = players.FirstOrDefault(i => i.photonView.IsMine);

            /* Update UI elements for myself first */
            sliderHealth.value = myPlayer.photonView.GetHealth() / (float)myPlayer.MaxHealth;

            sliderMana.value = myPlayer.photonView.GetMana() / (float)myPlayer.MaxMana;

            textHealth.text = $"{myPlayer.photonView.GetHealth()}/{myPlayer.MaxHealth}";

            textMana.text = $"{myPlayer.photonView.GetMana()}/{myPlayer.MaxMana}";

            /* Update UI elements that is tied-up to other players */
            var team1 = players.Where(i => i.photonView.GetTeam() == 0).ToArray();

            var team2 = players.Where(i => i.photonView.GetTeam() == 1).ToArray();

            for (int team = 0; team < Constants.MAX_TEAM; team++)
            {
                for (int i = 0; i < Constants.MAX_PLAYER_COUNT_PER_TEAM; i++)
                {
                    /* Handle for team 1 */
                    if (team == 0)
                    {
                        var player = i < team1.Count() ? team1[i] : null;

                        team1PlayerIndicators[i].sprite = player?.SpriteIcon ?? null;

                        team1PlayerIndicators[i].gameObject.SetActive(player != null);

                        team1ChestIndicators[i].gameObject.SetActive(player != null && player.photonView.HasChest());

                        team1ShipHuds[i].Player = player;
                    }

                    /* Handle for team 2 */
                    if (team == 1)
                    {
                        var player = i < team2.Count() ? team2[i] : null;

                        team2PlayerIndicators[i].sprite = player?.SpriteIcon ?? null;

                        team2PlayerIndicators[i].gameObject.SetActive(player != null);

                        team2ChestIndicators[i].gameObject.SetActive(player != null && player.photonView.HasChest());

                        team2ShipHuds[i].Player = player;
                    }
                }
            }

            /* Update player current gold UI */
            textPlayerGold.text = PhotonNetwork.LocalPlayer.GetGold().ToString();
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
                if(score[i] > int.Parse(teamScore[i].text))
                    teamScore[i].GetComponent<Animator>().Play("Animation");

                //assign score value to text
                teamScore[i].text = score[i].ToString();
            }
        }



        /// <summary>
        /// Sets death text showing who killed the player in its team color.
        /// Parameters: killer's name, killer's team
        /// </summary>
        public void SetDeathText(string playerName, Team team)
        {
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
            //clear text component values
            deathText.text = string.Empty;
            spawnDelayText.text = string.Empty;
        }


        /// <summary>
        /// Set game end text and display winning team in its team color.
        /// </summary>
        public void SetGameOverText(Team team)
        {
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
            SceneManager.LoadScene(Constants.MENU_SCENE_NAME);
        }
    }
}
