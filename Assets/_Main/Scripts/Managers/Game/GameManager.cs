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
using TanksMP;

public class GameManager : MonoBehaviourPun
{
    public static GameManager Instance;

    [SerializeField]
    public BaseManager[] bases;

    [SerializeField]
    public UIGame ui;

    [SerializeField]
    public Team[] teams;

    private List<Player> ships;

    private List<SupremacyWardEffectManager> supremacyWards;

    private List<GameEntityManager> entities;

    public List<Player> Ships { get => ships; }

    public List<Player> Team1Ships { get => ships.Where(i => i.GetTeam() == 0).ToList(); }

    public List<Player> Team2Ships { get => ships.Where(i => i.GetTeam() == 1).ToList(); }

    public List<SupremacyWardEffectManager> SupremacyWards { get => supremacyWards; }

    public List<GameEntityManager> Entities { get => entities; }

    #region Unity

    private void Awake()
    {
        Instance = this;

        ships = new List<Player>();

        supremacyWards = new List<SupremacyWardEffectManager>();

        entities = new List<GameEntityManager>();
    }

    private void Start()
    {
        AudioManager.Instance.PlayMusic(1);
    }

    #endregion

    #region Public

    public BaseManager GetBase(int team)
    {
        return bases.FirstOrDefault(i => i.Team == team);
    }

    public void CacheSupremacyWard(SupremacyWardEffectManager supremacyWard)
    {
        supremacyWards.Add(supremacyWard);
    }

    public void UncacheSupremacyWard(SupremacyWardEffectManager supremacyWard)
    {
        supremacyWards.Remove(supremacyWard);
    }

    public void CacheEntity(GameEntityManager entity)
    {
        entities.Add(entity);

        if (entity is Player)
        {
            ships.Add(entity as Player);
        }
    }

    public void UncacheEntity(GameEntityManager entity)
    {
        entities.Remove(entity);

        if (entity is Player)
        {
            ships.Remove(entity as Player);
        }
    }

    public void PlayerSurrender()
    {
        Player.Mine.HasSurrendered(true);
    }

    public void AddScore(ScoreType scoreType, int teamIndex)
    {
        switch (scoreType)
        {
            case ScoreType.Capture:
                PhotonNetwork.CurrentRoom.AddScore(teamIndex, 10, true);

                GPRewardSystem.m_instance.AddGoldToAllTeam(teamIndex, "Chest");
                break;

            case ScoreType.Kill:
                PhotonNetwork.CurrentRoom.AddScore(teamIndex, 1, false);
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
            var teamShips = ships.Where(ship => ship.GetTeam() == i);

            var teamSurrendered = teamShips.Count(i => i.HasSurrendered()) > teamShips.Count(i => !i.HasSurrendered());

            if (teamSurrendered) isOver = true;

            teamResults[i] = teamSurrendered ? BattleResultType.Defeat : BattleResultType.Victory;
        }

        // Decide if the game has to stop
        for (int i = 0; i < teams.Length; i++)
        {
            if(score[i] >= SOManager.Instance.Constants.ScoreRequired)
            {
                isOver = true;
            }
        }

        if (TimerManager.Instance.TimeLapse >= SOManager.Instance.Constants.GameTimer)
        {
            isOver = true;
        }

        //return the result
        return isOver;
    }

    public void DisplayGameOver(int winnerTeamIndex)
    {
        Player.Mine.enabled = false;

        //ui.OpenAftermath(winnerTeamIndex >= 0 ? teams[winnerTeamIndex] : null, winnerTeamIndex);

        AftermathUI.Instance.Open((self) =>
        {
            self.Data = new List<List<Player>> { Team1Ships, Team2Ships };

            self.BattleResult =
                winnerTeamIndex == -1 ? BattleResultType.Draw :
                winnerTeamIndex == PhotonNetwork.LocalPlayer.GetTeam() ? BattleResultType.Victory :
                BattleResultType.Defeat;
        });
    }

    #endregion
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