using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class ShopThreadInfo : DecisionThreadInfo
{
    public override DecisionNodeInfo GetFinalDecision(List<GameEntityManager> entities)
    {
        DecisionNodeInfo currentDecision = null;

        /* Only allow shop for bots when they are at their bases */
        if (GameManager.Instance.GetBase(player.GetTeam()).HasPlayer(player))
        {
            Debug.Log("[ShopThreadInfo] " + player.gameObject.name + " can buy an item.");

            foreach (var item in ShopManager.Instance.Data)
            {
                if (!ShopManager.Instance.CanBuy(player, item)) continue;

                currentDecision = GetBetterDecision(currentDecision, GetDecisionTo(item));
            }
        }

        return currentDecision;
    }

    private DecisionNodeInfo GetDecisionTo(ItemSO item)
    {
        var weight = GetWeightToItem(item);

        return new DecisionNodeInfo
        {
            Key = "Buy Item : " + item.Name,

            Weight = weight,

            Decision = () =>
            {
                ShopManager.Instance.Buy(player, item);
            }
        };
    }

    private float GetWeightToItem(ItemSO item)
    {
        var healthRatio = item.StatModifier.BuffMaxHealth / maxItemStatValues.Health * personality.GetWeightBuyItem("healthRatio");

        var manaRatio = item.StatModifier.BuffMaxMana / maxItemStatValues.Mana * personality.GetWeightBuyItem("manaRatio");

        var attackDamageRatio = item.StatModifier.BuffAttackDamage / maxItemStatValues.AttackDamage * personality.GetWeightBuyItem("attackDamageRatio");

        var abilityPowerRatio = item.StatModifier.BuffAbilityPower / maxItemStatValues.AbilityPower * personality.GetWeightBuyItem("abilityPowerRatio");

        var armorRatio = item.StatModifier.BuffArmor / maxItemStatValues.Armor * personality.GetWeightBuyItem("armorRatio");

        var resistRatio = item.StatModifier.BuffResist / maxItemStatValues.Resist * personality.GetWeightBuyItem("resistRatio");

        var attackSpeedRatio = item.StatModifier.BuffAttackSpeed / maxItemStatValues.AttackSpeed * personality.GetWeightBuyItem("attackSpeedRatio");

        var moveSpeedRatio = item.StatModifier.BuffMoveSpeed / maxItemStatValues.MoveSpeed * personality.GetWeightBuyItem("moveSpeedRatio");

        var lifeStealRatio = item.StatModifier.LifeSteal / maxItemStatValues.LifeSteal * personality.GetWeightBuyItem("lifeStealRatio");

        var cooldownRatio = item.StatModifier.BuffCooldown / maxItemStatValues.Cooldown * personality.GetWeightBuyItem("cooldownRatio");

        var invisibilityRatio = (item.StatModifier.IsInvisible ? 1f : 0f) * personality.GetWeightBuyItem("invisibilityRatio");

        /* Need to add some limit because if not, bot will always prioritize buying the cheapest item
         * despite it can afford the item that is really helpful. */
        var costRatio = Mathf.Min(1f, player.Inventory.Gold / item.CostBuy) * personality.GetWeightBuyItem("costRatio");

        var consumableRatio = (item.Category == CategoryType.Consumables ? 1f : 0f) * personality.GetWeightBuyItem("consumableRatio");

        return
            healthRatio + manaRatio + attackDamageRatio + abilityPowerRatio + armorRatio + 
            resistRatio + attackSpeedRatio + moveSpeedRatio + lifeStealRatio + cooldownRatio + 
            invisibilityRatio + costRatio + consumableRatio;
    }
}
