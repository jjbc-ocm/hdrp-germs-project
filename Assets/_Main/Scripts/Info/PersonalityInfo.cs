using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PersonalityInfo
{
    [SerializeField]
    private List<PriorityInfo> moveToPlayerPriorities;

    [SerializeField]
    private List<PriorityInfo> moveToMonsterPriorities;

    [SerializeField]
    private List<PriorityInfo> moveToChestPriorities;

    [SerializeField]
    private List<PriorityInfo> moveToCollectiblePriorities;

    [SerializeField]
    private List<PriorityInfo> moveToBasePriorities;

    [SerializeField]
    private List<PriorityInfo> buyItemPriorities;

    public float GetWeightMoveToPlayerPriority(string key)
    {
        return moveToPlayerPriorities.FirstOrDefault(i => i.Key == key).Weight;
    }

    public float GetWeightMoveToMonsterPriority(string key)
    {
        return moveToMonsterPriorities.FirstOrDefault(i => i.Key == key).Weight;
    }

    public float GetWeightMoveToChestPriority(string key)
    {
        return moveToChestPriorities.FirstOrDefault(i => i.Key == key).Weight;
    }

    public float GetWeightMoveToCollectiblePriority(string key)
    {
        return moveToCollectiblePriorities.FirstOrDefault(i => i.Key == key).Weight;
    }

    public float GetWeightMoveToBasePriority(string key)
    {
        return moveToBasePriorities.FirstOrDefault(i => i.Key == key).Weight;
    }

    public float GetWeightBuyItem(string key)
    {
        return buyItemPriorities.FirstOrDefault(i => i.Key == key).Weight;
    }
}
