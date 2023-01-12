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
        public static GameManager Instance;

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

        private SupremacyWardEffectManager[] supremacyWards;

        private PlayerOfflineSaveState[] offlineSaveStates;

        public Player[] Ships { get => ships; }

        public List<Player> Team1Ships { get => ships.Where(i => i.photonView.GetTeam() == 0).ToList(); }

        public List<Player> Team2Ships { get => ships.Where(i => i.photonView.GetTeam() == 1).ToList(); }

        public SupremacyWardEffectManager[] SupremacyWards { get => supremacyWards; }

        public PlayerOfflineSaveState[] OfflineSaveStates { get => offlineSaveStates; }


        //initialize variables
        void Awake()
        {
            Instance = this;
        }

        void Update()
        {
            ships = FindObjectsOfType<Player>();

            supremacyWards = FindObjectsOfType<SupremacyWardEffectManager>();

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
        
        // TODO: for now, do not use this since it will cause a massive bug
        public void CreateOrUpdateOfflineSaveState(Player player, out bool isReconnection)
        {
            var currentOfflineSaveState = offlineSaveStates != null && offlineSaveStates.Length > 0
                    ? offlineSaveStates.FirstOrDefault(i => i.UserId == player.photonView.Owner.UserId)
                    : null;

            // This will happen when player joins the game
            if (currentOfflineSaveState == null)
            {
                var offlineSaveState = Instantiate(prefabPlayerOfflineSaveState);

                offlineSaveState.Initialize(player, false);

                isReconnection = false;
            }

            // This will happen when player reconnects to the game after being disconnected
            else
            {
                currentOfflineSaveState.Initialize(player, true);

                isReconnection = true;
            }
        }

        public void PlayerSurrender()
        {
            Player.Mine.photonView.HasSurrendered(true);
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
        public bool IsGameOver(out BattleResultType[] teamResults)
        {
            //init variables
            teamResults = new BattleResultType[teams.Length];

            var isOver = false;

            var score = PhotonNetwork.CurrentRoom.GetScore();

            //loop over teams to find the highest score
            for (var i = 0; i < teams.Length; i++)
            {
                teamResults[i] = BattleResultType.Victory;

                // Decide winner by score
                for (var j = 0; j < teams.Length; j++)
                {
                    if (i == j) continue;
                    if (score[j] > score[i]) teamResults[i] = BattleResultType.Defeat;
                    if (score[j] == score[i]) teamResults[i] = BattleResultType.Draw;
                }

                // Decide winner by surrenders
                var teamShips = ships.Where(ship => ship.photonView.GetTeam() == i);

                var teamSurrendered = teamShips.Count(i => i.photonView.HasSurrendered()) > teamShips.Count(i => !i.photonView.HasSurrendered());

                if (teamSurrendered) isOver = true;

                teamResults[i] = teamSurrendered ? BattleResultType.Defeat : BattleResultType.Victory;
            }

            // Decide if the game has to stop
            for (int i = 0; i < teams.Length; i++)
            {
                if (score[i] >= Constants.SCORE_REQUIRED)
                {
                    isOver = true;
                    break;
                }
            }

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
        public void DisplayGameOver(int winnerTeamIndex)
        {
            Player.Mine.enabled = false;

            Player.Mine.CamFollow.HideMask(true);

            ui.OpenAftermath(winnerTeamIndex >= 0 ? teams[winnerTeamIndex] : null, winnerTeamIndex);

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