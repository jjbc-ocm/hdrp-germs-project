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
    public AudioClip m_chestOpenSFX;

    [Header("Misc.")]
    [SerializeField]
    private GameObject m_LoadIndicator;

    [Header("Chest Rewards Settings")]
    public GPChestRewardWindow m_rewardWindow;
    public GPGUIScreen m_rewardScreen;
    

    // Start is called before the first frame update
    void Start()
    {

    }

    public override void Show()
    {
        base.Show();

        TanksMP.AudioManager.Play2D(m_showScreenSFX);

        m_rewardCounterText.text = string.Format("Daily Login Rewards  <color=#f6e19c>{0} </color>/ {1}", m_currClaimedIdx, m_rewardCount);

        for (int i = 0; i < m_rewardBlocks.Count; i++)
        {
            m_rewardBlocks[i].ToggleChecked(i < m_currClaimedIdx);
            m_rewardBlocks[i].ToggleFocus(i == m_currClaimedIdx);
            m_rewardBlocks[i].SetDay(i+1);
            m_rewardBlocks[i].SetRewardSprite(m_rewards[i].m_rewardSO.m_rewardSprite);
            m_rewardBlocks[i].SetRewardAmount(m_rewards[i].m_amount);
        }
    }

    public override void Hide()
    {
        base.Hide();
    }

    public void ClaimReward()
    {
        GivePrize(m_rewards[m_currClaimedIdx]);
        m_currClaimedIdx++;
        m_currClaimedIdx = Mathf.Clamp(m_currClaimedIdx, 0, 7);
        TanksMP.AudioManager.Play2D(m_claimSFX);
        Hide();
    }

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
                    OpenChestsInSequence(chests);
                    break;
                }
            case GP_PRIZE_TYPE.kSilverChest:
                {
                    List<GPStoreChestSO> chests = new List<GPStoreChestSO>();
                    for (int i = 0; i < reward.m_amount; i++)
                    {
                        chests.Add(GPItemsDB.m_instance.m_silverChest);
                    }
                    OpenChestsInSequence(chests);
                    break;
                }
            case GP_PRIZE_TYPE.kGoldenChest:
                {
                    List<GPStoreChestSO> chests = new List<GPStoreChestSO>();
                    for (int i = 0; i < reward.m_amount; i++)
                    {
                        chests.Add(GPItemsDB.m_instance.m_goldenChest);
                    }
                    OpenChestsInSequence(chests);
                    break;
                }
            case GP_PRIZE_TYPE.kCrystalChest:
                {
                    List<GPStoreChestSO> chests = new List<GPStoreChestSO>();
                    for (int i = 0; i < reward.m_amount; i++)
                    {
                        chests.Add(GPItemsDB.m_instance.m_crystalChest);
                    }
                    OpenChestsInSequence(chests);
                    break;
                }
            default:
                break;
        }
    }

    public void OpenChestsInSequence(List<GPStoreChestSO> chests)
    {
        StartCoroutine(IEOpenChestsInSecuence(chests));
    }

    IEnumerator IEOpenChestsInSecuence(List<GPStoreChestSO> chests)
    {
        for (int i = 0; i < chests.Count; i++)
        {
            OpenChest(chests[i]);
            yield return new WaitForSeconds(3.0f);
            if (i < chests.Count - 1) // so the last one doesn't play a sound at the end
            {
                TanksMP.AudioManager.Play2D(m_chestOpenSFX);
            }
        }
    }

    public void OpenChest(GPStoreChestSO chestDesc)
    {
        GPGivenRewards rewards = chestDesc.OpenChest();
        m_rewardScreen.Show();
        m_rewardWindow.Show();
        m_rewardWindow.ClearContent();
        m_rewardWindow.DisplayChestImage(chestDesc);
        m_rewardWindow.DisplayCrewRewards(rewards.m_ships);
        m_rewardWindow.DisplayIconRewards(rewards.m_profileIcons);
        m_rewardWindow.DisplayDummyRewards(rewards.m_dummyParts);
        StartCoroutine(CloseRewardWindow()); // for now close reward window after 3 seconds
    }

    IEnumerator CloseRewardWindow()
    {
        yield return new WaitForSeconds(3.0f);
        m_rewardScreen.Hide();
        m_rewardWindow.Hide();
    }

}
