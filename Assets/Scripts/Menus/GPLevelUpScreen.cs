using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GPLevelUpScreen : GPGUIScreen
{
    [Header("Reward settings")]
    public Transform m_rewardFramesHolder;
    public GPRewardFrame m_rewardFramePrefab;
    public List<GPPrize> m_rewards;
    public GPChestOpeningLogic m_chestOpeningLogic;

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

        GiveRewards();

        //Play effects
        m_punchTween.PunchEffect();
        AudioManager.Instance.Play2D(m_showSFX);

        //Display the new current level on the UI
        m_levelText.text = APIManager.Instance.PlayerData.Level.ToString();

        //clear old rewards
        foreach (Transform child in m_rewardFramesHolder.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        //display rewards
        for (int i = 0; i < m_rewards.Count; i++)
        {
            GPRewardFrame frame = Instantiate(m_rewardFramePrefab, m_rewardFramesHolder);
            frame.DisplayReward(m_rewards[i].m_desc, m_rewards[i].m_prizeAmount);
        }
    }

    public override void Hide()
    {
        base.Hide();
    }

    /// <summary>
    /// Hides the screen and plays a SFX.
    /// </summary>
    public void ContinueButtonPressed()
    {
        Hide();
        AudioManager.Instance.Play2D(m_continueClickedSFX);
    }

    /// <summary>
    /// Gives all the level up rewards to the player.
    /// </summary>
    public async void GiveRewards()
    {
        m_LoadIndicator.SetActive(true);
        foreach (var reward in m_rewards)
        {
            switch (reward.m_desc.m_type)
            {
                case GP_PRIZE_TYPE.kGold:
                    await APIManager.Instance.PlayerData.AddCoins(reward.m_prizeAmount);
                    break;
                case GP_PRIZE_TYPE.kGems:
                    await APIManager.Instance.PlayerData.AddGems(reward.m_prizeAmount);
                    break;
                case GP_PRIZE_TYPE.kEnergy:
                    GPPlayerProfile.m_instance.AddEnergy(reward.m_prizeAmount);
                    break;
                case GP_PRIZE_TYPE.kWoodenChest:
                    {
                        List<GPStoreChestSO> m_chests = new List<GPStoreChestSO>();
                        for (int i = 0; i < reward.m_prizeAmount; i++)
                        {
                            m_chests.Add(GPItemsDB.m_instance.m_woodenChest);
                        }
                        m_chestOpeningLogic.OpenChestsInSequence(m_chests);
                        break;
                    }
                case GP_PRIZE_TYPE.kGoldenChest:
                    {
                        List<GPStoreChestSO> m_chests = new List<GPStoreChestSO>();
                        for (int i = 0; i < reward.m_prizeAmount; i++)
                        {
                            m_chests.Add(GPItemsDB.m_instance.m_goldenChest);
                        }
                        m_chestOpeningLogic.OpenChestsInSequence(m_chests);
                        break;
                    }
                case GP_PRIZE_TYPE.kSilverChest:
                    {
                        List<GPStoreChestSO> m_chests = new List<GPStoreChestSO>();
                        for (int i = 0; i < reward.m_prizeAmount; i++)
                        {
                            m_chests.Add(GPItemsDB.m_instance.m_silverChest);
                        }
                        m_chestOpeningLogic.OpenChestsInSequence(m_chests);
                        break;
                    }
                case GP_PRIZE_TYPE.kCrystalChest:
                    {
                        List<GPStoreChestSO> m_chests = new List<GPStoreChestSO>();
                        for (int i = 0; i < reward.m_prizeAmount; i++)
                        {
                            m_chests.Add(GPItemsDB.m_instance.m_crystalChest);
                        }
                        m_chestOpeningLogic.OpenChestsInSequence(m_chests);
                        break;
                    }
                default:
                    break;
            }
        }
        m_LoadIndicator.SetActive(false);
    }
}
