
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
    private AudioClip clip;

    private float timer;

    public int Team { get => team; }

    public AudioClip Clip { get => clip; }

    public float Timer { get => timer; }

    #region Unity

    private void Update()
    {
        var allyShips = team == 0 ? GameManager.Instance.Team1Ships : GameManager.Instance.Team2Ships;

        var enemyShips = team == 0 ? GameManager.Instance.Team2Ships : GameManager.Instance.Team1Ships;

        var hasChest = allyShips.Any(i => i.HasChest() && HasPlayer(i));

        var hasEnemy = enemyShips.Any(i => HasPlayer(i));

        if (hasChest)
        {
            if (!hasEnemy)
            {
                timer += Time.deltaTime;

                Debug.Log("[Drop Chest] No Enemy, Team: " + team + " Time: " + timer);

                if (timer >= SOManager.Instance.Constants.CaptureChestTime)
                {
                    DropChest();
                    timer = 0;
                }
            }
            else
            {
                Debug.Log("[Drop Chest] Has Enemy, Team: " + team + " Time: " + timer);
            }
        }
        else
        {
            timer = 0;
        }

        indicatorNormal.SetActive(!hasChest);

        indicatorCollect.SetActive(hasChest);
    }

    #endregion

    #region Public

    public bool HasPlayer(Player player)
    {
        return Vector3.Distance(collectibleZone.transform.position, player.transform.position) <= collectibleZone.transform.localScale.x / 2f;
    }

    #endregion

    #region Private

    private void DropChest()
    {
        if (clip) AudioManager.Instance.Play3D(clip, transform.position);

        /* Add score */
        GameManager.Instance.AddScore(ScoreType.Capture, team);

        /* Reset chest references */
        foreach (var ship in GameManager.Instance.Ships)
        {
            ship.HasChest(false);
        }

        if (GameManager.Instance.IsGameOver(out List<BattleResultType> teamResults))
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;

            Player.Mine.photonView.RPC("RpcGameOver", RpcTarget.All, teamResults.IndexOf(BattleResultType.Victory));

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