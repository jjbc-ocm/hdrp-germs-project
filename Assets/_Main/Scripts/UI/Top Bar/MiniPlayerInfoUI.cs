using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniPlayerInfoUI : UI<MiniPlayerInfoUI>
{
    [SerializeField]
    private Image imageShip;

    [SerializeField]
    private Image imageChest;

    [SerializeField]
    private Image imageKey;

    [SerializeField]
    private Slider sliderHealth;

    [SerializeField]
    private Slider sliderMana;

    public PlayerManager Data { get; set; }

    protected override void OnRefreshUI()
    {
        imageShip.sprite = Data.SoundVisuals.SpriteIcon;

        imageChest.gameObject.SetActive(Data.Stat.HasChest);

        imageKey.gameObject.SetActive(Data.Stat.HasKey);

        sliderHealth.value = Data.Stat.Health / (float)Data.Stat.MaxHealth();

        sliderMana.value = Data.Stat.Mana / (float)Data.Stat.MaxMana();
    }
}
