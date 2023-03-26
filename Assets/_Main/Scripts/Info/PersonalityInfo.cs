using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PersonalityInfo
{
    [SerializeField]
    private List<PriorityInfo> moveToAllyPlayerConditions;

    [SerializeField]
    private List<PriorityInfo> moveToEnemyPlayerConditions;

    [SerializeField]
    private List<PriorityInfo> moveToMonsterConditions;

    [SerializeField]
    private List<PriorityInfo> moveToKeyConditions;

    [SerializeField]
    private List<PriorityInfo> moveToChestConditions;

    [SerializeField]
    private List<PriorityInfo> moveToCollectibleConditions;

    [SerializeField]
    private List<PriorityInfo> moveToAllyBaseConditions;

    [SerializeField]
    private List<PriorityInfo> moveToEnemyBaseConditions;


    /*[SerializeField]
    private List<PriorityInfo> moveToPlayerPriorities;

    [SerializeField]
    private List<PriorityInfo> moveToMonsterPriorities;

    [SerializeField]
    private List<PriorityInfo> moveToChestPriorities;

    [SerializeField]
    private List<PriorityInfo> moveToCollectiblePriorities;

    [SerializeField]
    private List<PriorityInfo> moveToBasePriorities;*/

    [SerializeField]
    private List<PriorityInfo> buyItemPriorities;

    /*public List<PriorityInfo> MoveToPlayerPriorities { get => moveToPlayerPriorities; }

    public List<PriorityInfo> MoveToMonsterPriorities { get => moveToMonsterPriorities; }

    public List<PriorityInfo> MoveToChestPriorities { get => moveToChestPriorities; }

    public List<PriorityInfo> MoveToCollectiblePriorities { get => moveToCollectiblePriorities; }

    public List<PriorityInfo> MoveToBasePriorities { get => moveToBasePriorities; }

    public List<PriorityInfo> BuyItemPriorities { get => buyItemPriorities; }*/

    public List<PriorityInfo> MoveToAllyPlayerConditions { get => moveToAllyPlayerConditions; }

    public List<PriorityInfo> MoveToEnemyPlayerConditions { get => moveToEnemyPlayerConditions; }

    public List<PriorityInfo> MoveToMonsterConditions { get => moveToMonsterConditions; }

    public List<PriorityInfo> MoveToKeyConditions { get => moveToKeyConditions; }

    public List<PriorityInfo> MoveToChestConditions { get => moveToChestConditions; }

    public List<PriorityInfo> MoveToCollectibleConditions { get => moveToCollectibleConditions; }

    public List<PriorityInfo> MoveToAllyBaseConditions { get => moveToAllyBaseConditions; }

    public List<PriorityInfo> MoveToEnemyBaseConditions { get => moveToEnemyBaseConditions; }

    /*public float GetWeightMoveToPlayerPriority(string key)
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
    }*/

    public float GetWeightBuyItem(string key)
    {
        return buyItemPriorities.FirstOrDefault(i => i.Key == key).Weight;
    }
}
