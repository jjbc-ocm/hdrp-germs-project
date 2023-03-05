using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AllItemsUI : ListViewUI<ShopItemUI, AllItemsUI>
{
    [SerializeField]
    private GameObject buttonSelectedIndicator;

    [SerializeField]
    private GameObject[] indicatorSelectedStatTypes;

    public StatFilterType StatFilter { get; set; }

    public bool IsSelected { get; set; }

    protected override void OnRefreshUI()
    {
        buttonSelectedIndicator.SetActive(IsSelected);

        DeleteItems();

        RefreshItems(ShopManager.Instance.Data.Where(i => IsInStatFilter(i)), (item, data) =>
        {
            item.Data = data;
        });

        foreach (var indicatorSelectedStatType in indicatorSelectedStatTypes)
        {
            indicatorSelectedStatType.SetActive(false);
        }

        var statFilterIndex = ((int)StatFilter) - 1;

        if (statFilterIndex >= 0)
        {
            indicatorSelectedStatTypes[statFilterIndex].SetActive(true);
        }
    }
    
    public void OnStatFilterClick(int statFilterIndex)
    {
        RefreshUI((self) =>
        {
            self.StatFilter = (StatFilterType)statFilterIndex;
        });
    }

    private bool IsInStatFilter(ItemSO item)
    {
        switch (StatFilter)
        {
            case StatFilterType.AttackDamage: 
                return item.StatModifier.BuffAttackDamage > 0;
            case StatFilterType.AbilityPower: 
                return item.StatModifier.BuffAbilityPower > 0;
            case StatFilterType.AttackSpeed: 
                return item.StatModifier.BuffAttackSpeed > 0;
            case StatFilterType.Armor: 
                return item.StatModifier.BuffArmor > 0;
            case StatFilterType.Resist: 
                return item.StatModifier.BuffResist > 0;
            case StatFilterType.MovementSpeed: 
                return item.StatModifier.BuffMoveSpeed > 0;
            case StatFilterType.Health: 
                return item.StatModifier.BuffMaxHealth > 0;
            case StatFilterType.Mana: 
                return item.StatModifier.BuffMaxMana > 0;
            case StatFilterType.Cooldown: 
                return item.StatModifier.BuffCooldown > 0;
            case StatFilterType.LifeSteal: 
                return item.StatModifier.LifeSteal > 0;
            default: return true;
        }
    }
}
