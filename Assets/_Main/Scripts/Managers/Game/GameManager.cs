
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

    private List<PlayerManager> ships;

    private List<SupremacyWardEffectManager> supremacyWards;

    private List<GameEntityManager> entities;

    private BattleResultType[] battleResults;

    public List<PlayerManager> Ships { get => ships; }

    public List<PlayerManager> Team1Ships { get => ships.Where(i => i.GetTeam() == 0).ToList(); }

    public List<PlayerManager> Team2Ships { get => ships.Where(i => i.GetTeam() == 1).ToList(); }

    public List<SupremacyWardEffectManager> SupremacyWards { get => supremacyWards; }

    public List<GameEntityManager> Entities { get => entities; }

    public BattleResultType[] BattleResults { get => battleResults; }

    #region Unity

    private void Awake()
    {
        Instance = this;

        ships = new List<PlayerManager>();

        supremacyWards = new List<SupremacyWardEffectManager>();

        entities = new List<GameEntityManager>();

        battleResults = new BattleResultType[SOManager.Instance.Constants.MaxTeam];
    }

    private void Start()
    {
        AudioManager.Instance.PlayMusic(1);
    }

    private void Update()
    {
        var input = InputManager.Instance;

        // Shop can only be accessed if chat is minimized and player is in the base
        if (input.IsShop && !ChatManager.Instance.UI.IsMaximized && GetBase(PlayerManager.Mine.GetTeam()).HasPlayer(PlayerManager.Mine))
        {
            ShopManager.Instance.ToggleShop();
        }

        // Score board can be toggled
        if (input.IsScoreBoard)
        {
            if (!ScoreBoardUI.Instance.gameObject.activeSelf)
            {
                ScoreBoardUI.Instance.Open((self) =>
                {
                    self.Data = new List<List<PlayerManager>>
                    {
                            Team1Ships,
                            Instance.Team2Ships
                    };
                });
            }
            else
            {
                ScoreBoardUI.Instance.Close();
            }
        }

        UpdateCrosshair();

        UpdateDebugKeys();
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

        if (entity is PlayerManager)
        {
            ships.Add(entity as PlayerManager);
        }
    }

    public void UncacheEntity(GameEntityManager entity)
    {
        entities.Remove(entity);

        if (entity is PlayerManager)
        {
            ships.Remove(entity as PlayerManager);
        }
    }

    public void PlayerSurrender()
    {
        PlayerManager.Mine.HasSurrendered(true);
    }

    public void AddScoreByKill(int teamGain)
    {
        PhotonNetwork.CurrentRoom.AddScoreByKill(teamGain, 1);
    }

    public void AddScoreByChest(int teamGain, int teamLose)
    {
        PhotonNetwork.CurrentRoom.AddScoreByChest(teamGain, teamLose, 10);

        GPRewardSystem.m_instance.AddGoldToAllTeam(teamGain, "Chest");
    }

    public bool IsGameOver()
    {
        var isOver = false;

        // Decide battle result by score
        var score0 = PhotonNetwork.CurrentRoom.GetScore(0);

        var score1 = PhotonNetwork.CurrentRoom.GetScore(1);

        battleResults[0] =
            score0 > score1 ? BattleResultType.Victory :
            score0 < score1 ? BattleResultType.Defeat :
            BattleResultType.Draw;

        battleResults[1] =
            score1 > score0 ? BattleResultType.Victory :
            score1 < score0 ? BattleResultType.Defeat :
            BattleResultType.Draw;

        // Decide battle result who surrendered
        // TODO:

        // Decide if the game has to stop
        if (TimerManager.Instance.TimeLapse >= SOManager.Instance.Constants.GameTimer ||
            score0 >= SOManager.Instance.Constants.ScoreRequired ||
            score1 >= SOManager.Instance.Constants.ScoreRequired)
        {
            isOver = true;
        }

        return isOver;
    }

    public void DisplayGameOver(int winnerTeamIndex)
    {
        PlayerManager.Mine.enabled = false;

        //ui.OpenAftermath(winnerTeamIndex >= 0 ? teams[winnerTeamIndex] : null, winnerTeamIndex);

        AftermathUI.Instance.Open((self) =>
        {
            self.Data = new List<List<PlayerManager>> { Team1Ships, Team2Ships };

            self.BattleResult =
                winnerTeamIndex == -1 ? BattleResultType.Draw :
                winnerTeamIndex == PhotonNetwork.LocalPlayer.GetTeam() ? BattleResultType.Victory :
                BattleResultType.Defeat;
        });
    }

    #endregion

    #region Private

    private void UpdateCrosshair()
    {
        var player = PlayerManager.Mine;

        var offset = Vector3.up * 2;

        var aimPosition = player.transform.position + PlayerManager.Mine.CameraFollow.forward * 999f;

        var targetPosition = aimPosition + offset;

        var fromPosition = player.transform.position + offset;

        var direction = (targetPosition - fromPosition).normalized;

        var maxDistance = SOManager.Instance.Constants.FogOrWarDistance;

        var layerMask = Utils.GetBulletHitMask(gameObject);

        direction = new Vector3(direction.x, 0, direction.z).normalized;

        targetPosition = fromPosition + direction * SOManager.Instance.Constants.FogOrWarDistance + offset;

        if (Physics.Raycast(fromPosition, direction, out RaycastHit hit, maxDistance, layerMask))
        {
            CrosshairUI.Instance.Target = hit.point;
        }
        else
        {
            CrosshairUI.Instance.Target = targetPosition;
        }
    }

    private void UpdateDebugKeys()
    {
        if (InputManager.Instance.IsDebugKey(0))
        {
            PlayerManager.Mine.photonView.RPC("RpcDamageHealth", RpcTarget.All, 9999, PlayerManager.Mine.photonView.ViewID);

            //PhotonNetwork.InstantiateRoomObject("Chest", transform.position, Quaternion.identity);
        }

        if (InputManager.Instance.IsDebugKey(1))
        {
            var ship = Team1Ships[0];

            ship.Stat.SetChest(ship.Stat.HasKey && !ship.Stat.HasChest);
            ship.Stat.SetKey(!ship.Stat.HasKey && !ship.Stat.HasChest);
        }

        if (InputManager.Instance.IsDebugKey(2))
        {
            var ship = Team1Ships[1];

            ship.Stat.SetChest(ship.Stat.HasKey && !ship.Stat.HasChest);
            ship.Stat.SetKey(!ship.Stat.HasKey && !ship.Stat.HasChest);
        }

        if (InputManager.Instance.IsDebugKey(3))
        {
            var ship = Team1Ships[2];

            ship.Stat.SetChest(ship.Stat.HasKey && !ship.Stat.HasChest);
            ship.Stat.SetKey(!ship.Stat.HasKey && !ship.Stat.HasChest);
        }

        if (InputManager.Instance.IsDebugKey(4))
        {
            var ship = Team2Ships[0];

            ship.Stat.SetChest(ship.Stat.HasKey && !ship.Stat.HasChest);
            ship.Stat.SetKey(!ship.Stat.HasKey && !ship.Stat.HasChest);
        }

        if (InputManager.Instance.IsDebugKey(5))
        {
            var ship = Team2Ships[1];

            ship.Stat.SetChest(ship.Stat.HasKey && !ship.Stat.HasChest);
            ship.Stat.SetKey(!ship.Stat.HasKey && !ship.Stat.HasChest);
        }

        if (InputManager.Instance.IsDebugKey(6))
        {
            var ship = Team2Ships[2];

            ship.Stat.SetChest(ship.Stat.HasKey && !ship.Stat.HasChest);
            ship.Stat.SetKey(!ship.Stat.HasKey && !ship.Stat.HasChest);
        }

        if (InputManager.Instance.IsDebugKey(7))
        {

        }

        if (InputManager.Instance.IsDebugKey(8))
        {

        }

        if (InputManager.Instance.IsDebugKey(9))
        {

        }
    }

    #endregion
}