
using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using TanksMP;
using System.Linq;

public class CollectibleZone : GameEntityManager
{
    [SerializeField]
    private int team;

    [SerializeField]
    private AudioClip clip;

    private float timer;

    public int Team { get => team; }

    public AudioClip Clip { get => clip; }

    public float Timer { get => timer; }

    private void Update()
    {
        var allyShips = team == 0 ? GameManager.Instance.Team1Ships : GameManager.Instance.Team2Ships;

        var enemyShips = team == 0 ? GameManager.Instance.Team2Ships : GameManager.Instance.Team1Ships;

        var hasChest = allyShips.Any(i => i.HasChest() && Vector3.Distance(transform.position, i.transform.position) <= transform.localScale.x / 2f);

        var hasEnemy = enemyShips.Any(i => Vector3.Distance(transform.position, i.transform.position) <= transform.localScale.x / 2f);

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
    }

    public void DropChest()
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

    protected override void OnTriggerEnterCalled(Collider col)
    {

    }

    protected override void OnTriggerExitCalled(Collider col)
    {

    }
}