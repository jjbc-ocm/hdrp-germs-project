using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TanksMP;

public class GPRewardGiver : MonoBehaviour
{
  public int m_reward = 0;
  public CollectibleTeam m_collectible;

  private void Start()
  {
    if (m_collectible)
    {
      m_collectible.OnCollectedEvent.AddListener(OnCollected);
    }
  }

  public void GiveReward(GPRewardTaker taker)
  {
    taker.AddBooty(m_reward);
  }

  public void OnCollected(TanksMP.Player player, GameObject obj)
  {
    GPRewardTaker taker = obj.GetComponent<GPRewardTaker>();
    GiveReward(taker);
  }
}
