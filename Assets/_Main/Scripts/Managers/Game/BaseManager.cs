
using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using TanksMP;
using System.Linq;

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
        
        var hasAllyWithChest = allyShips.Any(i => i.Stat.HasChest && HasPlayer(i));

        var enemyWithKey = enemyShips.FirstOrDefault(i => HasPlayer(i) && i.Stat.HasKey);

        // If allied ship carrying a chest is in this base
        if (hasAllyWithChest)
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
                    DropChest();

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

        indicatorNormal.SetActive(!hasAllyWithChest);

        indicatorCollect.SetActive(hasAllyWithChest);
    }

    #endregion

    #region Public

    public bool HasPlayer(PlayerManager player)
    {
        return Vector3.Distance(collectibleZone.transform.position, player.transform.position) <= collectibleZone.transform.localScale.x / 2f;
    }

    #endregion

    #region Private

    private void FetchChest(PlayerManager player)
    {
        if (clip) AudioManager.Instance.Play3D(clip, transform.position);

        // Remove key in player's possessiob
        player.Stat.SetKey(false);

        // Add chest in player's possession
        player.Stat.SetChest(true);
    }

    private void DropChest()
    {
        if (clip) AudioManager.Instance.Play3D(clip, transform.position);

        // Add score
        //GameManager.Instance.AddScore(ScoreType.Capture, team);
        GameManager.Instance.AddScoreByChest(team, team == 0 ? 1 : 0);

        // Remove chest in any player's possession
        foreach (var ship in GameManager.Instance.Ships)
        {
            ship.Stat.SetChest(false);
        }

        if (GameManager.Instance.IsGameOver())
        {
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