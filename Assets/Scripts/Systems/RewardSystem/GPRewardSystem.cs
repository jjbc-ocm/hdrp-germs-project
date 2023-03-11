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
    public GPCoinMovement m_coin1Prefab;
    public GPCoinMovement m_coin10Prefab;
    public float m_coinDispersionRadius = 10.0f;

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
    public void AddGoldToPlayer(ActorManager player, int amount)
    {
        //var ship = GameManager.Instance.Ships.FirstOrDefault(i => i.photonView.Owner == player);

        player.photonView.RPC("AddGold", RpcTarget.All, amount);

        //player.AddGold(amount);
    }

    /// <summary>
    /// Adds gold to a specific player. The gold amount will be searched using a key.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="key"></param>
    public void AddGoldToPlayer(ActorManager player, string key)
    {
        if (m_rewardsMap.ContainsKey(key))
        {
            //var ship = GameManager.Instance.Ships.FirstOrDefault(i => i.photonView.Owner == player);



            player.photonView.RPC("AddGold", RpcTarget.All, m_rewardsMap[key]);



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
        /*foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.GetTeam() == teamIndex)
            {
                AddGoldToPlayer(player, amount);
            }
        }*/

        foreach (var player in GameManager.Instance.Ships)
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
        /*foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.GetTeam() == teamIndex)
            {
                AddGoldToPlayer(player, key);
            }
        }*/

        foreach (var player in GameManager.Instance.Ships)
        {
            if (player.GetTeam() == teamIndex)
            {
                AddGoldToPlayer(player, key);
            }
        }
    }

    public int GetRewardAmountByKey(string key)
    {
        if (m_rewardsMap.ContainsKey(key))
        {
            return m_rewardsMap[key];
        }
        Debug.LogWarning("Key for reward not found.");
        return 0;
    }

    public void SpawnCoins(Vector3 position, int amount, Transform targetPlayer)
    {
        Vector3 offset = Vector3.zero;

        if (amount > 10)
        {
            int tens = amount / 10;
            amount -= tens * 10;
            for (int i = 0; i < tens; i++)
            {
                offset.x = UnityEngine.Random.Range(-m_coinDispersionRadius, m_coinDispersionRadius);
                offset.z = UnityEngine.Random.Range(-m_coinDispersionRadius, m_coinDispersionRadius);
                GPCoinMovement instancedItem = Instantiate(m_coin10Prefab, position, Quaternion.identity).GetComponent<GPCoinMovement>();
                instancedItem.m_target = targetPlayer;
                instancedItem.StartCoroutine(instancedItem.Dispersate(position + offset));
            }
        }

        for (int i = 0; i < amount; i++)
        {
            offset.x = UnityEngine.Random.Range(-m_coinDispersionRadius, m_coinDispersionRadius);
            offset.z = UnityEngine.Random.Range(-m_coinDispersionRadius, m_coinDispersionRadius);
            GPCoinMovement instancedItem = Instantiate(m_coin1Prefab, position, Quaternion.identity).GetComponent<GPCoinMovement>();
            instancedItem.m_target = targetPlayer;
            instancedItem.StartCoroutine(instancedItem.Dispersate(position + offset));
        }

    }

}
