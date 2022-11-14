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

        sliderHealth.value = myPlayer.Health / (float)myPlayer.MaxHealth;

        sliderMana.value = myPlayer.Mana / (float)myPlayer.MaxMana;

        textHealth.text = $"{myPlayer.Health}/{myPlayer.MaxHealth}";

        textMana.text = $"{myPlayer.Mana}/{myPlayer.MaxMana}";

        textStats.text = $"{ad}\n{ap}\n{@as}\n{ms}\n{ar}\n{mr}";
    }
}
