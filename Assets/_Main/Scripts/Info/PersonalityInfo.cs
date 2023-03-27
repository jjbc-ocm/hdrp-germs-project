using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PersonalityInfo
{
    [SerializeField]
    private List<MovePriorityInfo> moveToAllyPlayerConditions;

    [SerializeField]
    private List<MovePriorityInfo> moveToEnemyPlayerConditions;

    [SerializeField]
    private List<MovePriorityInfo> moveToMonsterConditions;

    [SerializeField]
    private List<MovePriorityInfo> moveToKeyConditions;

    [SerializeField]
    private List<MovePriorityInfo> moveToChestConditions;

    [SerializeField]
    private List<MovePriorityInfo> moveToCollectibleConditions;

    [SerializeField]
    private List<MovePriorityInfo> moveToAllyBaseConditions;

    [SerializeField]
    private List<MovePriorityInfo> moveToEnemyBaseConditions;

    [SerializeField]
    private List<BuyPriorityInfo> buyItemConditions;

    //[SerializeField]
    //private List<MovePriorityInfo> buyItemPriorities;

    public List<MovePriorityInfo> MoveToAllyPlayerConditions { get => moveToAllyPlayerConditions; }

    public List<MovePriorityInfo> MoveToEnemyPlayerConditions { get => moveToEnemyPlayerConditions; }

    public List<MovePriorityInfo> MoveToMonsterConditions { get => moveToMonsterConditions; }

    public List<MovePriorityInfo> MoveToKeyConditions { get => moveToKeyConditions; }

    public List<MovePriorityInfo> MoveToChestConditions { get => moveToChestConditions; }

    public List<MovePriorityInfo> MoveToCollectibleConditions { get => moveToCollectibleConditions; }

    public List<MovePriorityInfo> MoveToAllyBaseConditions { get => moveToAllyBaseConditions; }

    public List<MovePriorityInfo> MoveToEnemyBaseConditions { get => moveToEnemyBaseConditions; }

    public List<BuyPriorityInfo> BuyItemConditions { get => buyItemConditions; }

    /*public float GetWeightBuyItem(string key)
    {
        return buyItemPriorities.FirstOrDefault(i => i.Key == key).Weight;
    }*/
}
