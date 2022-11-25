using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TanksMP;
using System.Linq;

public class GPRewardSystem : MonoBehaviour
{
    [System.Serializable]
    public class RewardData
    {
        public string key;
        public int value;
    }

    public static GPRewardSystem m_instance;

    [Header("Gold settings")]
    public List<RewardData> m_rewardsData;
    public Dictionary<string, int> m_rewardsMap = new Dictionary<string, int>(); // cant set it from inspector, use m_rewardsData, values will be stored here.

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        foreach (var data in m_rewardsData)
        {
            m_rewardsMap.Add(data.key, data.value);
        }
    }

    /// <summary>
    /// Adds gold to a specific player.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="amount"></param>
    public void AddGoldToPlayer(Photon.Realtime.Player player, int amount)
    {
        var ship = GameManager.GetInstance().Ships.FirstOrDefault(i => i.photonView.Owner == player);

        ship.photonView.RPC("AddGold", RpcTarget.All, amount);

        //player.AddGold(amount);
    }

    /// <summary>
    /// Adds gold to a specific player. The gold amount will be searched using a key.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="key"></param>
    public void AddGoldToPlayer(Photon.Realtime.Player player, string key)
    {
        if (m_rewardsMap.ContainsKey(key))
        {
            var ship = GameManager.GetInstance().Ships.FirstOrDefault(i => i.photonView.Owner == player);

            ship.photonView.RPC("AddGold", RpcTarget.All, m_rewardsMap[key]);

            //player.AddGold(m_rewardsMap[key]);
        }
        else
        {
            Debug.LogWarning("Key for reward not found.");
        }
    }

    /// <summary>
    /// Adds gold to all members of a team.
    /// </summary>
    /// <param name="teamIndex"></param>
    /// <param name="amount"></param>
    public void AddGoldToAllTeam(int teamIndex, int amount)
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.GetTeam() == teamIndex)
            {
                AddGoldToPlayer(player, amount);
            }
        }
    }

    /// <summary>
    /// Adds gold to all members of a team. The gold amount will be searched using a key.
    /// </summary>
    /// <param name="teamIndex"></param>
    /// <param name="key"></param>
    public void AddGoldToAllTeam(int teamIndex, string key)
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.GetTeam() == teamIndex)
            {
                AddGoldToPlayer(player, key);
            }
        }
    }

}
