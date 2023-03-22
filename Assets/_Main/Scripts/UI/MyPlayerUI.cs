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
    private Button buttonShop;

    [SerializeField]
    private ItemSlotUI[] itemSlots;

    void Update()
    {
        RefreshUI();
    }

    protected override void OnRefreshUI()
    {
        if (PlayerManager.Mine == null) return;

        var ad = PlayerManager.Mine.Stat.AttackDamage();

        var ap = PlayerManager.Mine.Stat.AbilityPower();

        var @as = PlayerManager.Mine.Stat.AttackSpeed();

        var ms = PlayerManager.Mine.Stat.MoveSpeed();

        var ar = PlayerManager.Mine.Stat.Armor();

        var mr = PlayerManager.Mine.Stat.Resist();

        var attackCooldown = Mathf.Max(0, PlayerManager.Mine.NextAttackTime - Time.time);

        var skillCooldown = Mathf.Max(0,PlayerManager.Mine.NextSkillTime - Time.time);

        var attackMaxCooldown = SOManager.Instance.Constants.MoveSpeedToSecondsRatio / PlayerManager.Mine.Stat.AttackSpeed();

        var skillMaxCooldown = PlayerManager.Mine.Skill.Cooldown;

        imageShip.sprite = PlayerManager.Mine.Data.ShipIconImage;

        imageAttack.sprite = PlayerManager.Mine.Attack.Icon;

        imageSkill.sprite = PlayerManager.Mine.Skill.Icon;

        sliderHealth.value = PlayerManager.Mine.Stat.Health / (float)PlayerManager.Mine.Stat.MaxHealth();

        sliderMana.value = PlayerManager.Mine.Stat.Mana / (float)PlayerManager.Mine.Stat.MaxMana();

        textHealth.text = $"{PlayerManager.Mine.Stat.Health}/{PlayerManager.Mine.Stat.MaxHealth()}";

        textMana.text = $"{PlayerManager.Mine.Stat.Mana}/{PlayerManager.Mine.Stat.MaxMana()}";

        textStats.text = $"{ad}\n{ap}\n{@as}\n{ms}\n{ar}\n{mr}";

        textAttackCooldown.text = attackCooldown.ToString("F1");

        textSkillCooldown.text = skillCooldown.ToString("F1");

        textAttackCooldown.gameObject.SetActive(attackCooldown > 0);

        textSkillCooldown.gameObject.SetActive(skillCooldown > 0);

        imageAttack.fillAmount = (attackMaxCooldown - attackCooldown) / attackMaxCooldown;

        imageSkill.fillAmount = (skillMaxCooldown - skillCooldown) / skillMaxCooldown;

        buttonShop.interactable = GameManager.Instance.GetBase(PlayerManager.Mine.GetTeam()).HasPlayer(PlayerManager.Mine);

        var items = PlayerManager.Mine.Inventory.Items;

        if (items == null) return;

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
