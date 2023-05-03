
using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using TanksMP;
using System.Linq;
using UnityEngine.UI;

public class BaseManager : GameEntityManager
{
    [SerializeField]
    private int team;

    [SerializeField]
    private GameObject collectibleZone;

    [SerializeField]
    private GameObject indicatorNormal;

    [SerializeField]
    private GameObject indicatorCollect;

    [SerializeField]
    private GameObject spawnPoint;

    [SerializeField]
    private AudioClip clip;

    [Header("Chest Zone")]

    [SerializeField]
    private GameObject[] chests;

    [SerializeField]
    private Slider sliderChests;

    private float timer;

    public GameObject SpawnPoint { get => spawnPoint; }

    public int Team { get => team; }

    public AudioClip Clip { get => clip; }

    public float Timer { get => timer; }

    #region Unity

    private void Update()
    {
        var allyShips = team == 0 ? GameManager.Instance.Team1Ships : GameManager.Instance.Team2Ships;

        var enemyShips = team == 0 ? GameManager.Instance.Team2Ships : GameManager.Instance.Team1Ships;

        var allyWithChest = allyShips.FirstOrDefault(i => i.Stat.HasChest() && HasPlayer(i));

        UpdateChestLogic(enemyShips, allyWithChest);

        UpdateChestVisual(allyWithChest);
    }

    #endregion

    #region Public

    public bool HasPlayer(PlayerManager player)
    {
        return Vector3.Distance(collectibleZone.transform.position, player.transform.position) <= SOManager.Instance.Constants.BaseRadius;
    }

    public Vector3 GetSpawnPosition()
    {
        var offset = new Vector3(
            Random.value - 0.5f,
            0,
            Random.value - 0.5f);

        return spawnPoint.transform.position + offset;
    }

    public Quaternion GetSpawnRotation()
    {
        return spawnPoint.transform.rotation;
    }

    #endregion

    #region Private

    private void UpdateChestLogic(List<PlayerManager> enemyShips, PlayerManager allyWithChest)
    {
        var enemyWithKey = enemyShips.FirstOrDefault(i => HasPlayer(i) && i.Stat.HasKey);

        // If allied ship carrying a chest is in this base
        if (allyWithChest)
        {
            var hasEnemy = enemyShips.Any(i => HasPlayer(i));

            // If there's no enemy in this base
            if (!hasEnemy)
            {
                // Proceed on the countdown to drop the chest
                timer += Time.deltaTime;

                if (timer >= SOManager.Instance.Constants.CaptureChestTime)
                {
                    // Finally dropping the chest
                    DropChest(allyWithChest);

                    // Reset countdown so it has to repeat the same process next time
                    timer = 0;
                }
            }
        }

        // Reset timer always if there's no allied ship carrying a chest 
        else
        {
            timer = 0;
        }

        // If enemy ship carrying a key is in this base
        if (enemyWithKey)
        {
            FetchChest(enemyWithKey);
        }
    }

    private void UpdateChestVisual(PlayerManager allyWithChest)
    {
        indicatorNormal.SetActive(!allyWithChest);

        indicatorCollect.SetActive(allyWithChest);

        var chestCount = SOManager.Instance.Constants.MaxChestPerTeam - PhotonNetwork.CurrentRoom.GetChestLost(team);

        sliderChests.value = chestCount;

        for (var i = 0; i < chests.Length; i++)
        {
            chests[i].SetActive(i < chestCount);
        }
    }

    private void FetchChest(PlayerManager player)
    {
        if (clip) AudioManager.Instance.Play3D(clip, transform.position);

        // Remove key in player's possessiob
        player.Stat.SetKey(false);

        // Add chest in player's possession
        player.Stat.SetChest(true, team);
    }

    private void DropChest(PlayerManager allyWithChest)
    {
        if (clip) AudioManager.Instance.Play3D(clip, transform.position);

        // Add score
        if ((team == 0 && allyWithChest.Stat.HasChest(1)) || 
            (team == 1 && allyWithChest.Stat.HasChest(0)))
        {
            GameManager.Instance.AddScoreByChest(team, team == 0 ? 1 : 0);
        }
        
        // Remove chest in any player's possession
        foreach (var ship in GameManager.Instance.Ships)
        {
            ship.Stat.SetChest(false);
        }

        if (GameManager.Instance.IsGameOver())
        {
            Debug.Log("GOT IN BASE");

            PhotonNetwork.CurrentRoom.IsOpen = false;

            PlayerManager.Mine.photonView.RPC("RpcGameOver", RpcTarget.All);

            return;
        }
    }

    #endregion

    protected override void OnTriggerEnterCalled(Collider col)
    {

    }

    protected override void OnTriggerExitCalled(Collider col)
    {

    }
}