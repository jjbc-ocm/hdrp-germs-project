/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using System.Collections;
using UnityEngine;
using Photon.Pun;
using System;
namespace TanksMP
{
    /// <summary>
    /// Manages game workflow and provides high-level access to networked logic during a game.
    /// It manages functions such as team fill, scores and ending a game, but also video ad results.
    /// </summary>
    public class GameManager : MonoBehaviourPun
    {
        //reference to this script instance
        private static GameManager instance;

        /// <summary>
        /// The local player instance spawned for this client.
        /// </summary>
        [HideInInspector]
        public Player localPlayer;

        /// <summary>
        /// This is just temporary, because timer should be decided by the team first. Just use this for now for testing
        /// Default is 10 min only for testing
        /// </summary>
        public double gameTimer = 600;

        /// <summary>
        /// Added by: Jilmer John
        /// Use this as reference for mainCamera
        /// </summary>
        public Camera mainCamera;

        /// <summary>
        /// Added by: Jilmer John
        /// </summary>
        public CollectibleZone zoneRed;

        /// <summary>
        /// Added by: Jilmer John
        /// </summary>
        public CollectibleZone zoneBlue;

        /// <summary>
        /// Active game mode played in the current scene.
        /// </summary>
        //public GameMode gameMode = GameMode.CaptureTheChest;

        /// <summary>
        /// Reference to the UI script displaying game stats.
        /// </summary>
        public UIGame ui;

        /// <summary>
        /// Definition of playing teams with additional properties.
        /// </summary>
        public Team[] teams;

        /// <summary>
        /// The maximum amount of kills to reach before ending the game.
        /// </summary>
        public int maxScore = 30;

        /// <summary>
        /// The delay in seconds before respawning a player after it got killed.
        /// </summary>
        public int respawnTime = 5;

        /// <summary>
        /// Enable or disable friendly fire. This is verified in the Bullet script on collision.
        /// </summary>
        public bool friendlyFire = false;


        //initialize variables
        void Awake()
        {
            instance = this;
        }

        void Update()
        {
            // Update fog-of-war for the local player
            var players = FindObjectsOfType<Player>();

            foreach (var player in players)
            {
                if (player.photonView.GetTeam() != localPlayer.photonView.GetTeam())
                {
                    var distance = Vector3.Distance(player.transform.position, localPlayer.transform.position);

                    player.IconIndicator.SetActive(distance <= Constants.FOG_OF_WAR_DISTANCE);
                }
            }
        }


        /// <summary>
        /// Returns a reference to this script instance.
        /// </summary>
        public static GameManager GetInstance()
        {
            return instance;
        }
        
        
        /// <summary>
        /// Global check whether this client is the match master or not.
        /// </summary>
        /*public static bool isMaster()
        {
            return PhotonNetwork.IsMasterClient;
        }*/


        /// <summary>
        /// Returns the next team index a player should be assigned to.
        /// </summary>
        /*public int GetTeamFill()
        {
            //init variables
            int[] size = PhotonNetwork.CurrentRoom.GetSize();
            int teamNo = 0;

            int min = size[0];
            //loop over teams to find the lowest fill
            for (int i = 0; i < teams.Length; i++)
            {
                //if fill is lower than the previous value
                //store new fill and team for next iteration
                if (size[i] < min)
                {
                    min = size[i];
                    teamNo = i;
                }
            }

            //return index of lowest team
            return teamNo;
        }*/


        /// <summary>
        /// Returns a random spawn position within the team's spawn area.
        /// </summary>
        public Vector3 GetSpawnPosition(int teamIndex)
        {
            //init variables
            Vector3 pos = teams[teamIndex].spawn.position;
            BoxCollider col = teams[teamIndex].spawn.GetComponent<BoxCollider>();

            if(col != null)
            {
                //find a position within the box collider range, first set fixed y position
                //the counter determines how often we are calculating a new position if out of range
                pos.y = col.transform.position.y;
                int counter = 10;
                
                //try to get random position within collider bounds
                //if it's not within bounds, do another iteration
                do
                {
                    pos.x = UnityEngine.Random.Range(col.bounds.min.x, col.bounds.max.x);
                    pos.z = UnityEngine.Random.Range(col.bounds.min.z, col.bounds.max.z);
                    counter--;
                }
                while(!col.bounds.Contains(pos) && counter > 0);
            }
            
            return pos;
        }


        /// <summary>
        /// Adds points to the target team depending on matching game mode and score type.
        /// This allows us for granting different amount of points on different score actions.
        /// </summary>
        public void AddScore(ScoreType scoreType, int teamIndex)
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("mode", out object mode))
            {
                var intMode = Convert.ToInt32(mode);

                var enumMode = (GameMode)intMode;

                switch (enumMode)
                {
                    case GameMode.CaptureTheChest:

                        switch (scoreType)
                        {
                            case ScoreType.Capture:
                                PhotonNetwork.CurrentRoom.AddScore(teamIndex, 10);

                                GPRewardSystem.m_instance.AddGoldToAllTeam(teamIndex, "Chest");
                                break;

                            case ScoreType.Kill:
                                // If killed an enemy, give score of coins?
                                break;
                        }
                        break;

                    case GameMode.DeathMatch:
                        switch (scoreType)
                        {
                            case ScoreType.Kill:
                                PhotonNetwork.CurrentRoom.AddScore(teamIndex, 1);
                                break;
                        }
                        break;
                }
            }
            
        }
        

        /// <summary>
        /// Returns whether a team reached the maximum game score.
        /// </summary>
        public bool IsGameOver()
        {
            //init variables
            bool isOver = false;
            int[] score = PhotonNetwork.CurrentRoom.GetScore();
            
            //loop over teams to find the highest score
            for(int i = 0; i < teams.Length; i++)
            {
                //score is greater or equal to max score,
                //which means the game is finished
                if(score[i] >= maxScore)
                {
                    isOver = true;
                    break;
                }
            }

            // if maximum time is reached
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("mode", out object mode))
            {
                var intMode = Convert.ToInt32(mode);

                var enumMode = (GameMode)intMode;

                if (enumMode == GameMode.Survival && Timer.Instance.TimeLapse >= gameTimer)
                {
                    isOver = true;
                }
            }
            
            //return the result
            return isOver;
        }
        
        /*public void SpawnPlayer(PhotonView photonView)
        {
            StartCoroutine(SpawnRoutine(photonView));
        }

        private IEnumerator SpawnRoutine(PhotonView photonView)
        {
            float targetTime = Time.time + 3;

            while (targetTime - Time.time > 0)
            {
                ui.SetSpawnDelay(targetTime - Time.time);

                yield return null;
            }

            ui.DisableDeath();

            photonView.RPC("RpcRespawn", RpcTarget.All);
        }*/

        /// <summary>
        /// Only for this player: sets the death text stating the killer on death.
        /// If Unity Ads is enabled, tries to show an ad during the respawn delay.
        /// By using the 'skipAd' parameter is it possible to force skipping ads.
        /// </summary>
        /*public void DisplayDeath(bool skipAd = false) // TODO: Bug might be here
        {
            Debug.Log("DisplayDeath A");

            //get the player component that killed us
            Player other = localPlayer;
            string killedByName = "YOURSELF";
            if(localPlayer.killedBy != null)
                other = localPlayer.killedBy.GetComponent<Player>();

            Debug.Log("DisplayDeath B");

            //suicide or regular kill?
            if (other != localPlayer)
            {
                killedByName = other.photonView.GetName();
                //increase local death counter for this game
                //ui.killCounter[1].text = (int.Parse(ui.killCounter[1].text) + 1).ToString();
                //ui.killCounter[1].GetComponent<Animator>().Play("Animation");

                Debug.Log("DisplayDeath C");
            }

            //when no ad is being shown, set the death text
            //and start waiting for the respawn delay immediately
            Debug.Log("DisplayDeath D");
            ui.SetDeathText(killedByName, teams[other.photonView.GetTeam()]);
            Debug.Log("DisplayDeath E");
            StartCoroutine(SpawnRoutine());
        }*/


        //coroutine spawning the player after a respawn delay
        /*IEnumerator SpawnRoutine()
        {
            Debug.Log("SpawnRoutine A");
            //calculate point in time for respawn
            float targetTime = Time.time + respawnTime;

            Debug.Log("SpawnRoutine B");
            //wait for the respawn to be over,
            //while waiting update the respawn countdown
            while (targetTime - Time.time > 0)
            {
                ui.SetSpawnDelay(targetTime - Time.time);
                yield return null;
            }

            Debug.Log("SpawnRoutine C");
            //respawn now: send request to the server
            ui.DisableDeath();
            Debug.Log("SpawnRoutine D");
            //localPlayer.CmdRespawn();
        }*/


        /// <summary>
        /// Only for this player: sets game over text stating the winning team.
        /// Disables player movement so no updates are sent through the network.
        /// </summary>
        public void DisplayGameOver(int teamIndex)
        {
            //PhotonNetwork.isMessageQueueRunning = false;
            localPlayer.enabled = false;
            localPlayer.camFollow.HideMask(true);
            ui.SetGameOverText(teams[teamIndex]);

            //starts coroutine for displaying the game over window
            //StopCoroutine(SpawnRoutine(null));
            StartCoroutine(DisplayGameOver());
        }


        //displays game over window after short delay
        IEnumerator DisplayGameOver()
        {
            //give the user a chance to read which team won the game
            //before enabling the game over screen
            yield return new WaitForSeconds(3);

            //show game over window (still connected at that point)
            ui.ShowGameOver();
        }


        //clean up callbacks on scene switches
        void OnDestroy()
        {
            #if UNITY_ADS
                UnityAdsManager.adResultEvent -= HandleAdResult;
            #endif
        }
    }


    /// <summary>
    /// Defines properties of a team.
    /// </summary>
    [System.Serializable]
    public class Team
    {
        /// <summary>
        /// The name of the team shown on game over.
        /// </summary>
        public string name;

        /// <summary>
        /// The color of a team for UI and player prefabs.
        /// </summary>   
        public Material material;

        /// <summary>
        /// The spawn point of a team in the scene. In case it has a BoxCollider
        /// component attached, a point within the collider bounds will be used.
        /// </summary>
        public Transform spawn;
    }


    /// <summary>
    /// Defines the types that could grant points to players or teams.
    /// Used in the AddScore() method for filtering.
    /// </summary>
    public enum ScoreType
    {
        Kill,
        Capture
    }


    /// <summary>
    /// Available game modes selected per scene.
    /// Used in the AddScore() method for filtering.
    /// </summary>
    public enum GameMode
    {
        CaptureTheChest,

        DeathMatch,

        Survival
    }
}