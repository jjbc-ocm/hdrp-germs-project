using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopThreadInfo : DecisionThreadInfo
{
    public override DecisionNodeInfo GetFinalDecision(GameEntityManager[] entities)
    {
        DecisionNodeInfo currentDecision = null;

        foreach (var item in ShopManager.Instance.Data)
        {
            if (!ShopManager.Instance.CanBuy(player, item)) continue;

            currentDecision = GetBetterDecision(currentDecision, GetDecisionTo(item));
        }
        return currentDecision;
    }

    private DecisionNodeInfo GetDecisionTo(ItemData item)
    {
        return new DecisionNodeInfo
        {
            Key = "Buy Item : " + item.Name,

            Weight = GetWeightToItem(item),

            Decision = () =>
            {
                ShopManager.Instance.Buy(player, item);
            }
        };
    }

    private float GetWeightToItem(ItemData item)
    {
        var healthRatio = item.StatModifier.BuffMaxHealth / maxItemStatValues.Health;

        var manaRatio = item.StatModifier.BuffMaxMana / maxItemStatValues.Mana;

        var attackDamageRatio = item.StatModifier.BuffAttackDamage / maxItemStatValues.AttackDamage;

        var abilityPowerRatio = item.StatModifier.BuffAbilityPower / maxItemStatValues.AbilityPower;

        var armorRatio = item.StatModifier.BuffArmor / maxItemStatValues.Armor;

        var resistRatio = item.StatModifier.BuffResist / maxItemStatValues.Resist;

        var attackSpeedRatio = item.StatModifier.BuffAttackSpeed / maxItemStatValues.AttackSpeed;

        var moveSpeedRatio = item.StatModifier.BuffMoveSpeed / maxItemStatValues.MoveSpeed;

        var lifeStealRatio = item.StatModifier.LifeSteal / maxItemStatValues.LifeSteal;

        var cooldownRatio = item.StatModifier.BuffCooldown / maxItemStatValues.Cooldown;

        var invisibilityRatio = item.StatModifier.IsInvisible ? 1f : 0f;

        var costRatio = 1 - item.CostBuy / maxItemStatValues.Cost;


        return
            healthRatio * personality.GetWeightBuyItem("healthRatio") +
            manaRatio * personality.GetWeightBuyItem("manaRatio") +
            attackDamageRatio * personality.GetWeightBuyItem("attackDamageRatio") +
            abilityPowerRatio * personality.GetWeightBuyItem("abilityPowerRatio") +
            armorRatio * personality.GetWeightBuyItem("armorRatio") +
            resistRatio * personality.GetWeightBuyItem("resistRatio") +
            attackSpeedRatio * personality.GetWeightBuyItem("attackSpeedRatio") +
            moveSpeedRatio * personality.GetWeightBuyItem("moveSpeedRatio") +
            lifeStealRatio * personality.GetWeightBuyItem("lifeStealRatio") +
            cooldownRatio * personality.GetWeightBuyItem("cooldownRatio") +
            invisibilityRatio * personality.GetWeightBuyItem("invisibilityRatio") +
            costRatio * personality.GetWeightBuyItem("costRatio");
    }
}
