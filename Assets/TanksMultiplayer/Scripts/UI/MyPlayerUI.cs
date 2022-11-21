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

    private Player myPlayer;

    void Update()
    {
        RefreshUI((self) =>
        {
            if (myPlayer == null)
            {
                var players = FindObjectsOfType<Player>();

                myPlayer = players.FirstOrDefault(i => i.photonView.IsMine);
            }
        });
    }

    protected override void OnRefreshUI()
    {
        if (myPlayer == null) return;

        var photonView = myPlayer.photonView;

        var ad = photonView.GetAttackDamage();

        var ap = photonView.GetAbilityPower();

        var @as = photonView.GetAttackSpeed();

        var ms = photonView.GetMoveSpeed();

        var ar = photonView.GetArmor();

        var mr = photonView.GetResist();

        imageAttack.sprite = myPlayer.Attack.Icon;

        imageSkill.sprite = myPlayer.Skill.Icon;

        sliderHealth.value = myPlayer.Stat.Health / (float)myPlayer.Stat.MaxHealth;

        sliderMana.value = myPlayer.Stat.Mana / (float)myPlayer.Stat.MaxMana;

        textHealth.text = $"{myPlayer.Stat.Health}/{myPlayer.Stat.MaxHealth}";

        textMana.text = $"{myPlayer.Stat.Mana}/{myPlayer.Stat.MaxMana}";

        textStats.text = $"{ad}\n{ap}\n{@as}\n{ms}\n{ar}\n{mr}";

        var items = photonView.GetItems(ShopManager.Instance.Data.ToArray());

        /* For item slots */
        var slotId = 0;

        for (var i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].gameObject.SetActive(false);
        }

        foreach (var item in items)
        {
            for (var i = 0; i < item.Count; i++)
            {
                if (slotId == itemSlots.Length) break;

                itemSlots[slotId].gameObject.SetActive(true);

                itemSlots[slotId].RefreshUI((self) =>
                {
                    self.Data = item.Item;
                });

                slotId++;
            }
        }
    }
}
