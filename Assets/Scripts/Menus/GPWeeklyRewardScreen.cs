using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GPWeeklyRewardScreen : GPGUIScreen
{
    [System.Serializable]
    public class WeeklyReward
    {
        public GPRewardSO m_rewardSO;
        public int m_amount = 0;
    }

    [Header("Reward settings")]
    public TextMeshProUGUI m_rewardCounterText;
    public List<GPWeeklyRewardBlock> m_rewardBlocks;
    [Tooltip("Prizes to give each day in order. Only the first 7 will be used (one per day).")]
    public List<WeeklyReward> m_rewards;

    [HideInInspector]
    public int m_currClaimedIdx = 0;
    public int m_rewardCount = 7;

    [Header("Audio settings")]
    public AudioClip m_showScreenSFX;
    public AudioClip m_claimSFX;

    [Header("Misc.")]
    [SerializeField]
    private GameObject m_LoadIndicator;

    [Header("Chest Rewards Settings")]
    public GPChestOpeningLogic m_chestOpeningLogic;

    // Start is called before the first frame update
    void Start()
    {

    }

    public override void Show()
    {
        base.Show();

        TanksMP.AudioManager.Play2D(m_showScreenSFX);

        UpdateDisplayedData();
    }

    public override void Hide()
    {
        base.Hide();
    }

    /// <summary>
    /// Gives the rewards to the player, play effects and updates the claimed reward number.
    /// </summary>
    public void ClaimReward()
    {
        GivePrize(m_rewards[m_currClaimedIdx]);
        m_currClaimedIdx++;
        m_currClaimedIdx = Mathf.Clamp(m_currClaimedIdx, 0, 7);
        TanksMP.AudioManager.Play2D(m_claimSFX);
        Hide();
    }

    /// <summary>
    /// Resets the current reward to claim to be the first one.
    /// </summary>
    public void ResetWeekProgress()
    {
        m_currClaimedIdx = 0;
        UpdateDisplayedData();
    }

    /// <summary>
    /// Displays the reward sprites, eneables check marks on the claimed ones,
    /// displays the number of claimed rewards and enables the focus sprite on
    /// the current reward block.
    /// </summary>
    void UpdateDisplayedData()
    {
        m_rewardCounterText.text = string.Format("Daily Login Rewards  <color=#f6e19c>{0} </color>/ {1}", m_currClaimedIdx, m_rewardCount);

        for (int i = 0; i < m_rewardBlocks.Count; i++)
        {
            m_rewardBlocks[i].ToggleChecked(i < m_currClaimedIdx);
            m_rewardBlocks[i].SetDay(i + 1);
            m_rewardBlocks[i].ToggleFocus(i == m_currClaimedIdx);
            m_rewardBlocks[i].SetRewardSprite(m_rewards[i].m_rewardSO.m_rewardSprite);
            m_rewardBlocks[i].SetRewardAmount(m_rewards[i].m_amount);
        }
    }

    /// <summary>
    /// Gives the given weekly reward to the player.
    /// </summary>
    /// <param name="reward"></param>
    public async void GivePrize(WeeklyReward reward)
    {
        switch (reward.m_rewardSO.m_type)
        {
            case GP_PRIZE_TYPE.kGold:
                m_LoadIndicator.SetActive(true);
                await APIManager.Instance.PlayerData.AddCoins(reward.m_amount);
                m_LoadIndicator.SetActive(false);
                break;
            case GP_PRIZE_TYPE.kGems:
                m_LoadIndicator.SetActive(true);
                await APIManager.Instance.PlayerData.AddGems(reward.m_amount);
                m_LoadIndicator.SetActive(false);
                break;
            case GP_PRIZE_TYPE.kEnergy:
                GPPlayerProfile.m_instance.AddEnergy(reward.m_amount);
                break;
            case GP_PRIZE_TYPE.kWoodenChest:
                {
                    List<GPStoreChestSO> chests = new List<GPStoreChestSO>();
                    for (int i = 0; i < reward.m_amount; i++)
                    {
                        chests.Add(GPItemsDB.m_instance.m_woodenChest);
                    }
                    m_chestOpeningLogic.OpenChestsInSequence(chests);
                    break;
                }
            case GP_PRIZE_TYPE.kSilverChest:
                {
                    List<GPStoreChestSO> chests = new List<GPStoreChestSO>();
                    for (int i = 0; i < reward.m_amount; i++)
                    {
                        chests.Add(GPItemsDB.m_instance.m_silverChest);
                    }
                    m_chestOpeningLogic.OpenChestsInSequence(chests);
                    break;
                }
            case GP_PRIZE_TYPE.kGoldenChest:
                {
                    List<GPStoreChestSO> chests = new List<GPStoreChestSO>();
                    for (int i = 0; i < reward.m_amount; i++)
                    {
                        chests.Add(GPItemsDB.m_instance.m_goldenChest);
                    }
                    m_chestOpeningLogic.OpenChestsInSequence(chests);
                    break;
                }
            case GP_PRIZE_TYPE.kCrystalChest:
                {
                    List<GPStoreChestSO> chests = new List<GPStoreChestSO>();
                    for (int i = 0; i < reward.m_amount; i++)
                    {
                        chests.Add(GPItemsDB.m_instance.m_crystalChest);
                    }
                    m_chestOpeningLogic.OpenChestsInSequence(chests);
                    break;
                }
            default:
                break;
        }
    }

}
