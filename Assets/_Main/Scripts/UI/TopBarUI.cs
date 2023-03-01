using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopBarUI : UI<TopBarUI>
{
    [SerializeField]
    private Image[] team1PlayerIndicators;

    [SerializeField]
    private Image[] team2PlayerIndicators;

    [SerializeField]
    private Image[] team1ChestIndicators;

    [SerializeField]
    private Image[] team2ChestIndicators;

    [SerializeField]
    private Slider[] team1HealthSliders;

    [SerializeField]
    private Slider[] team2HealthSliders;

    [SerializeField]
    private Slider[] team1ManaSliders;

    [SerializeField]
    private Slider[] team2ManaSliders;

    [SerializeField]
    private TMP_Text textKills;

    [SerializeField]
    private TMP_Text textDeaths;

    void Update()
    {
        RefreshUI();
    }

    protected override void OnRefreshUI()
    {
        /* Update UI elements that is tied-up to other players */
        var team1 = GameManager.Instance.Team1Ships;

        var team2 = GameManager.Instance.Team2Ships;

        for (int team = 0; team < SOManager.Instance.Constants.MaxTeam; team++)
        {
            for (int i = 0; i < SOManager.Instance.Constants.MaxPlayerPerTeam; i++)
            {
                /* Handle for team 1 */
                if (team == 0)
                {
                    var player = i < team1.Count() ? team1[i] : null;

                    team1PlayerIndicators[i].sprite = player?.SoundVisuals.SpriteIcon ?? null;

                    team1PlayerIndicators[i].gameObject.SetActive(player != null);

                    team1ChestIndicators[i].gameObject.SetActive(player != null && player.HasChest());

                    team1HealthSliders[i].gameObject.SetActive(player != null);

                    team1ManaSliders[i].gameObject.SetActive(player != null);

                    if (player != null)
                    {
                        team1HealthSliders[i].value = player.Stat.Health / (float)player.Stat.MaxHealth();

                        team1ManaSliders[i].value = player.Stat.Mana / (float)player.Stat.MaxMana();
                    }
                }

                /* Handle for team 2 */
                if (team == 1)
                {
                    var player = i < team2.Count() ? team2[i] : null;

                    team2PlayerIndicators[i].sprite = player?.SoundVisuals.SpriteIcon ?? null;

                    team2PlayerIndicators[i].gameObject.SetActive(player != null);

                    team2ChestIndicators[i].gameObject.SetActive(player != null && player.HasChest());

                    team2HealthSliders[i].gameObject.SetActive(player != null);

                    team2ManaSliders[i].gameObject.SetActive(player != null);

                    if (player != null)
                    {
                        team2HealthSliders[i].value = player.Stat.Health / (float)player.Stat.MaxHealth();

                        team2ManaSliders[i].value = player.Stat.Mana / (float)player.Stat.MaxMana();
                    }
                }
            }
        }

        if (Player.Mine == null) return;

        textKills.text = Player.Mine.Stat.Kills.ToString();

        textDeaths.text = Player.Mine.Stat.Deaths.ToString();
    }
}
