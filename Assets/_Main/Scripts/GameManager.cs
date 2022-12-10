/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using System.Collections;
using UnityEngine;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;

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

        [SerializeField]
        private PlayerOfflineSaveState prefabPlayerOfflineSaveState;

        /// <summary>
        /// Added by: Jilmer John
        /// </summary>
        public CollectibleZone zoneRed;

        /// <summary>
        /// Added by: Jilmer John
        /// </summary>
        public CollectibleZone zoneBlue;

        /// <summary>
        /// Reference to the UI script displaying game stats.
        /// </summary>
        public UIGame ui;

        /// <summary>
        /// Definition of playing teams with additional properties.
        /// </summary>
        public Team[] teams;

        private Player[] ships;

        private PlayerOfflineSaveState[] offlineSaveStates;

        public Player[] Ships { get => ships; }

        public PlayerOfflineSaveState[] OfflineSaveStates { get => offlineSaveStates; }


        //initialize variables
        void Awake()
        {
            instance = this;
        }

        void Update()
        {
            ships = FindObjectsOfType<Player>();

            offlineSaveStates = FindObjectsOfType<PlayerOfflineSaveState>();

            foreach (var ship in ships)
            {
                if (Player.Mine != null && ship.photonView.GetTeam() != Player.Mine.photonView.GetTeam())
                {
                    var distance = Vector3.Distance(ship.transform.position, Player.Mine.transform.position);

                    ship.SoundVisuals.IconIndicator.SetActive(distance <= Constants.FOG_OF_WAR_DISTANCE);
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
        
        public void CreateOrUpdateOfflineSaveState(Player player)
        {
            var currentOfflineSaveState = offlineSaveStates != null && offlineSaveStates.Length > 0
                    ? offlineSaveStates.FirstOrDefault(i => i.UserId == player.photonView.Owner.UserId)
                    : null;

            if (currentOfflineSaveState == null)
            {
                var offlineSaveState = Instantiate(prefabPlayerOfflineSaveState);

                offlineSaveState.Initialize(player, false);
            }
            else
            {
                currentOfflineSaveState.Initialize(player, true);
            }
        }


        /// <summary>
        /// Adds points to the target team depending on matching game mode and score type.
        /// This allows us for granting different amount of points on different score actions.
        /// </summary>
        public void AddScore(ScoreType scoreType, int teamIndex)
        {
            switch (scoreType)
            {
                case ScoreType.Capture:
                    PhotonNetwork.CurrentRoom.AddScore(teamIndex, 10);

                    GPRewardSystem.m_instance.AddGoldToAllTeam(teamIndex, "Chest");
                    break;

                case ScoreType.Kill:
                    PhotonNetwork.CurrentRoom.AddScore(teamIndex, 1);
                    break;
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
                if(score[i] >= Constants.SCORE_REQUIRED)
                {
                    isOver = true;
                    break;
                }
            }

            // if maximum time is reached
            if (TimerManager.Instance.TimeLapse >= Constants.GAME_MAX_TIMER)
            {
                isOver = true;
            }

            //return the result
            return isOver;
        }

        /// <summary>
        /// Only for this player: sets game over text stating the winning team.
        /// Disables player movement so no updates are sent through the network.
        /// </summary>
        public void DisplayGameOver(int teamIndex)
        {
            //PhotonNetwork.isMessageQueueRunning = false;
            Player.Mine.enabled = false;
            Player.Mine.CamFollow.HideMask(true);
            ui.SetGameOverText(teams[teamIndex]);

            //starts coroutine for displaying the game over window
            //StopCoroutine(SpawnRoutine(null));
            StartCoroutine(YieldDisplayGameOver(teamIndex));
        }


        //displays game over window after short delay
        IEnumerator YieldDisplayGameOver(int teamIndex)
        {
            //give the user a chance to read which team won the game
            //before enabling the game over screen
            yield return new WaitForSeconds(3);

            //show game over window (still connected at that point)
            ui.ShowGameOver(teamIndex);
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
        //public Transform spawn;
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
}