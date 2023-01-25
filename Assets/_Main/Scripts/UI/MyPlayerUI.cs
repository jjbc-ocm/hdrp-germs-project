using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyPlayerUI : UI<MyPlayerUI>
{
    [SerializeField]
    private Image imageShip;

    [SerializeField]
    private Image imageAttack;

    [SerializeField]
    private Image imageSkill;

    [SerializeField]
    private Slider sliderHealth;

    [SerializeField]
    private Slider sliderMana;

    [SerializeField]
    private TMP_Text textHealth;

    [SerializeField]
    private TMP_Text textMana;

    [SerializeField]
    private TMP_Text textStats;

    [SerializeField]
    private TMP_Text textAttackCooldown;

    [SerializeField]
    private TMP_Text textSkillCooldown;

    [SerializeField]
    private ItemSlotUI[] itemSlots;

    void Update()
    {
        RefreshUI();
    }

    protected override void OnRefreshUI()
    {
        if (Player.Mine == null) return;

        var ad = Player.Mine.Stat.AttackDamage;

        var ap = Player.Mine.Stat.AbilityPower;

        var @as = Player.Mine.Stat.AttackSpeed;

        var ms = Player.Mine.Stat.MoveSpeed;

        var ar = Player.Mine.Stat.Armor;

        var mr = Player.Mine.Stat.Resist;

        var attackCooldown = Mathf.Max(0, Player.Mine.NextAttackTime - Time.time);

        var skillCooldown = Mathf.Max(0,Player.Mine.NextSkillTime - Time.time);

        imageShip.sprite = Player.Mine.Data.ShipIconImage;

        imageAttack.sprite = Player.Mine.Attack.Icon;

        imageSkill.sprite = Player.Mine.Skill.Icon;

        sliderHealth.value = Player.Mine.Stat.Health / (float)Player.Mine.Stat.MaxHealth;

        sliderMana.value = Player.Mine.Stat.Mana / (float)Player.Mine.Stat.MaxMana;

        textHealth.text = $"{Player.Mine.Stat.Health}/{Player.Mine.Stat.MaxHealth}";

        textMana.text = $"{Player.Mine.Stat.Mana}/{Player.Mine.Stat.MaxMana}";

        textStats.text = $"{ad}\n{ap}\n{@as}\n{ms}\n{ar}\n{mr}";

        textAttackCooldown.text = attackCooldown.ToString("F1");

        textSkillCooldown.text = skillCooldown.ToString("F1");

        textAttackCooldown.gameObject.SetActive(attackCooldown > 0);

        textSkillCooldown.gameObject.SetActive(skillCooldown > 0);

        imageAttack.fillAmount = (Player.Mine.Attack.Cooldown - attackCooldown) / (Constants.MOVE_SPEED_TO_SECONDS_RATIO / Player.Mine.Stat.AttackSpeed);

        imageSkill.fillAmount = (Player.Mine.Skill.Cooldown - skillCooldown) / Player.Mine.Skill.Cooldown;

        var items = Player.Mine.Inventory.Items;

        /* For item slots */

        for (var i = 0; i < itemSlots.Length; i++)
        {
            var item = items[i];

            itemSlots[i].RefreshUI((self) =>
            {
                self.Index = i;

                self.Data = item;
            });
        }
    }
}
