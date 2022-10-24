using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPRewardTaker : MonoBehaviour
{
  public int m_currentBooty = 0;

  public void SetBooty(int booty)
  {
    m_currentBooty = booty;
    OnBootyChanged();
  }

  public void AddBooty(int booty)
  {
    m_currentBooty += booty;
    OnBootyChanged();
  }

  public void SpendBooty(int booty)
  {
    m_currentBooty -= booty;
    OnBootyChanged();
  }

  public void OnBootyChanged()
  {
    m_currentBooty = Mathf.Clamp(m_currentBooty, 0 , int.MaxValue);
  }
}
