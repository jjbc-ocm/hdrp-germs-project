using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPRewardSystem : MonoBehaviour
{
  public static GPRewardSystem m_instance;

  public void AddBooty(int booty)
  {
    if (booty < 0)
    {
      Debug.LogWarning("Cant add negative booty");
      return;
    }

  }
}
