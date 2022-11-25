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
    private ItemSlotUI[] itemSlots;

    void Update()
    {
        RefreshUI();
    }

    protected override void OnRefreshUI()
    {
        var photonView = Player.Mine.photonView;

        var ad = photonView.GetAttackDamage();

        var ap = photonView.GetAbilityPower();

        var @as = photonView.GetAttackSpeed();

        var ms = photonView.GetMoveSpeed();

        var ar = photonView.GetArmor();

        var mr = photonView.GetResist();

        imageAttack.sprite = Player.Mine.Attack.Icon;

        imageSkill.sprite = Player.Mine.Skill.Icon;

        sliderHealth.value = Player.Mine.Stat.Health / (float)Player.Mine.Stat.MaxHealth;

        sliderMana.value = Player.Mine.Stat.Mana / (float)Player.Mine.Stat.MaxMana;

        textHealth.text = $"{Player.Mine.Stat.Health}/{Player.Mine.Stat.MaxHealth}";

        textMana.text = $"{Player.Mine.Stat.Mana}/{Player.Mine.Stat.MaxMana}";

        textStats.text = $"{ad}\n{ap}\n{@as}\n{ms}\n{ar}\n{mr}";

        var items = Player.Mine.Inventory.GetAllItems();

        /* For item slots */

        for (var i = 0; i < itemSlots.Length; i++)
        {
            var item = items[i];

            itemSlots[i].gameObject.SetActive(item != null);

            if (item != null)
            {
                itemSlots[i].RefreshUI((self) =>
                {
                    self.Data = item;
                });
            }
            
        }
    }
}
