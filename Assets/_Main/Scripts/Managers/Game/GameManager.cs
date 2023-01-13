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
    public class GameManager : MonoBehaviourPun
    {
        public static GameManager Instance;

        [SerializeField]
        private PlayerOfflineSaveState prefabPlayerOfflineSaveState;

        [SerializeField]
        public CollectibleZone zoneRed;

        [SerializeField]
        public CollectibleZone zoneBlue;

        [SerializeField]
        public UIGame ui;

        [SerializeField]
        public Team[] teams;

        private Player[] ships;

        private SupremacyWardEffectManager[] supremacyWards;

        public Player[] Ships { get => ships; }

        public List<Player> Team1Ships { get => ships.Where(i => i.photonView.GetTeam() == 0).ToList(); }

        public List<Player> Team2Ships { get => ships.Where(i => i.photonView.GetTeam() == 1).ToList(); }

        public SupremacyWardEffectManager[] SupremacyWards { get => supremacyWards; }


        //initialize variables
        void Awake()
        {
            Instance = this;
        }

        void Update()
        {
            ships = FindObjectsOfType<Player>();

            supremacyWards = FindObjectsOfType<SupremacyWardEffectManager>();

            //offlineSaveStates = FindObjectsOfType<PlayerOfflineSaveState>();

            foreach (var ship in ships)
            {
                if (Player.Mine != null && ship.photonView.GetTeam() != Player.Mine.photonView.GetTeam())
                {
                    var distance = Vector3.Distance(ship.transform.position, Player.Mine.transform.position);

                    ship.SoundVisuals.IconIndicator.SetActive(distance <= Constants.FOG_OF_WAR_DISTANCE);
                }
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
        

        public bool IsGameOver(out List<BattleResultType> teamResults)
        {
            teamResults = new List<BattleResultType>();

            var isOver = false;

            var score = PhotonNetwork.CurrentRoom.GetScore();
            
            
            for (var i = 0; i < teams.Length; i++)
            {
                teamResults.Add(BattleResultType.Victory);

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

            // Decide winner by surrenders
            /*var team1Surrendered = Team1Ships.Count(i => i.photonView.HasSurrendered()) > Team1Ships.Count(i => !i.photonView.HasSurrendered());
            var team2Surrendered = Team2Ships.Count(i => i.photonView.HasSurrendered()) > Team2Ships.Count(i => !i.photonView.HasSurrendered());

            if (team1Surrendered || team2Surrendered) isOver = true;

            teamResults[0] = team1Surrendered ? BattleResultType.Defeat : BattleResultType.Victory;
            teamResults[1] = team2Surrendered ? BattleResultType.Defeat : BattleResultType.Victory;*/


            // Decide if the game has to stop
            for (int i = 0; i < teams.Length; i++)
            {
                if(score[i] >= Constants.SCORE_REQUIRED)
                {
                    isOver = true;
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