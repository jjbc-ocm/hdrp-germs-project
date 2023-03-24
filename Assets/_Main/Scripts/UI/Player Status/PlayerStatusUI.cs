using System.Collections;
using System.Collections.Generic;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : UI<PlayerStatusUI>
{
    [SerializeField]
    private Image imageShip;

    [SerializeField]
    private Image imageAttack;

    [SerializeField]
    private Image imageSkill;

    [SerializeField]
    private TMP_Text textName;

    [SerializeField]
    private TMP_Text textKills;

    [SerializeField]
    private TMP_Text textDeaths;

    [SerializeField]
    private TMP_Text textGold;

    [SerializeField]
    private ItemSlotUI[] itemSlots;

    public PlayerManager Data { get; set; }

    protected override void OnRefreshUI()
    {
        imageShip.sprite = Data.Data.ShipIconImage;

        imageAttack.sprite = Data.Attack.Icon;

        imageSkill.sprite = Data.Skill.Icon;

        textName.text = Data.GetName();

        textKills.text = Data.Stat.Kills.ToString();

        textDeaths.text = Data.Stat.Deaths.ToString();

        textGold.text = Data.Inventory.Gold.ToString();

        for (var i = 0; i < itemSlots.Length; i++)
        {
            var item = Data.Inventory.Items[i];

            itemSlots[i].RefreshUI((self) =>
            {
                self.Index = i;

                self.Data = item;
            });
        }
    }
}
