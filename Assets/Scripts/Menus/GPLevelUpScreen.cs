using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GPLevelUpScreen : GPGUIScreen
{
    [Header("Reward settings")]
    public GPPrize m_goldPrize;
    public GPPrize m_energyPrize;
    public GPPrize m_gemPrize;

    [Header("Animation settings")]
    public GPPunchTween m_punchTween;

    [Header("Audio Settings")]
    public AudioClip m_showSFX;
    public AudioClip m_continueClickedSFX;

    [Header("OTher references Settings")]
    public TextMeshProUGUI m_levelText;

    [Header("Misc.")]
    [SerializeField]
    private GameObject m_LoadIndicator;

    // Start is called before the first frame update
    void Start()
    {

    }

    public override void Show()
    {
        base.Show();
        m_punchTween.PunchEffect();
        GiveRewards();
        TanksMP.AudioManager.Play2D(m_showSFX);
        m_levelText.text = APIManager.Instance.PlayerData.Level.ToString();
    }

    public override void Hide()
    {
        base.Hide();
    }

    public void ContinueButtonPressed()
    {
        Hide();
        TanksMP.AudioManager.Play2D(m_continueClickedSFX);
    }

    public async void GiveRewards()
    {
        m_LoadIndicator.SetActive(true);
        await APIManager.Instance.PlayerData.AddCoins(m_goldPrize.m_prizeAmount);
        await APIManager.Instance.PlayerData.AddGems(m_gemPrize.m_prizeAmount);
        GPPlayerProfile.m_instance.AddEnergy(m_energyPrize.m_prizeAmount);
        m_LoadIndicator.SetActive(false);
    }
}
