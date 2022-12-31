using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GPRewardSO", menuName = "ScriptableObjects/GPRewardSO")]
public class GPRewardSO : ScriptableObject
{
    public string m_rewardName;
    public Sprite m_rewardSprite;
    public GP_PRIZE_TYPE m_type;
}
