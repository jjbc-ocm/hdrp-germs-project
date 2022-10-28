using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TanksMP;

public class GPRewardGiver : MonoBehaviour
{
    public int m_reward = 0;

    private void Start()
    {
    }

    public void GiveReward(GPRewardTaker taker)
    {
        taker.AddBooty(m_reward);
    }

    public void GiveReward(TanksMP.Player player)
    {
        player.photonView.AddGold(m_reward);
    }

    public void OnCollected(TanksMP.Player player, GameObject obj)
    {
        GPRewardTaker taker = obj.GetComponent<GPRewardTaker>();
        GiveReward(taker);
    }
}
