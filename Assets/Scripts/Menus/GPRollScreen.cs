using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KaimiraGames;
using TMPro;

public enum GP_PRIZE_TYPE
{
    kGold,
    kGems,
    kEnergy,
    kWoodenChest,
    kGoldenChest,
    kSilverChest,
    kCrystalChest,
}

[System.Serializable]
public class GPPrize
{
    public int m_prizeAmount;
    public GPRewardSO m_desc;
}

[System.Serializable]
public class GPWheelPrize : GPPrize
{
    public int m_weight;
}

public class GPRollScreen : GPGUIScreen
{
    public GPStoreScreen m_storeScreen;

    [Header("UI references")]
    public Button m_spinButton;
    public Transform m_wheelContent;
    public Transform m_wheelBackground;

    [Header("Prize settings")]
    [Tooltip("List them in clock wise order starting from the top of the wheel")]
    public List<GPWheelPrize> m_prizes;
    public List<GPRouletteItem> m_rouletteItemsSlots;
    WeightedList<string> m_weightedList = new();

    [Header("Prize chest refrences")]
    public GPStoreChestSO m_woodChest;
    public GPStoreChestSO m_goldenChest;

    [Header("Animation settings")]
    public int m_numberCirclestoRotate = 5;
    public float m_rotateTime = 3.0f;
    public AnimationCurve m_curve;
    float m_angleOnePrize;
    float m_circleAngles = 360.0f;
    float m_currentTime = 0.0f;
    bool m_spinning = false;
    public GPPunchTween m_startSpinTween;
    public GPPunchTween m_endSpinTween;

    [Header("Chest Rewards Settings")]
    public GPChestOpeningLogic m_chestOpeningLogic;

    [Header("Particle Settings")]
    public ParticleSystem m_winningParticlePrefab;

    [Header("Audio Settings")]
    public AudioClip m_spinStartedSFX;
    public AudioClip m_spinEndedSFX;

    [Header("Energy Settings")]
    public int m_spinWheelCost = 10;
    public Image m_energyFill;
    public TextMeshProUGUI m_energyAmountText;
    public float m_fillAnimSpeed = 7.0f;
    float m_fillTargetValue = 0.0f;

    [Header("Misc.")]

    [SerializeField]
    private GameObject m_LoadIndicator;

    void Start()
    {
        DisplayRewards();

        m_angleOnePrize = m_circleAngles / m_prizes.Count;
        m_spinButton.onClick.AddListener(Spin);
        OnEnergyUpdated();

        GPPlayerProfile.m_instance.OnEnergyModifiedEvent.AddListener(OnEnergyUpdated);
    }

    private void Update()
    {
        m_energyFill.fillAmount = Mathf.Lerp(m_energyFill.fillAmount, m_fillTargetValue, Time.deltaTime * m_fillAnimSpeed);
    }

    public void DisplayRewards()
    {
        for (int i = 0; i < m_prizes.Count; i++)
        {
            m_rouletteItemsSlots[i].SetSprite(m_prizes[i].m_desc.m_rewardSprite);
            m_rouletteItemsSlots[i].SetTextAmount(m_prizes[i].m_prizeAmount);
            m_weightedList.Add(m_prizes[i].m_desc.m_type.ToString() + " " + m_prizes[i].m_prizeAmount.ToString(),
                               m_prizes[i].m_weight);
        }
    }

    public override void Show()
    {
        base.Show();
        OnEnergyUpdated();
        ShuffleRewards();
    }

    public void Spin()
    {
        if (!m_spinning && GPPlayerProfile.m_instance.TrySpendEnergy(m_spinWheelCost))
        {
            m_spinning = true;
            m_spinButton.interactable = false;
            TanksMP.AudioManager.Play2D(m_spinStartedSFX);
            m_startSpinTween.PunchEffect();
            StartCoroutine(IESpin());
        }
    }

    IEnumerator IESpin()
    {
        m_storeScreen.LockButtons(true);
        string selectedPrize = m_weightedList.Next();
        int prizeIDX = m_weightedList.IndexOf(selectedPrize);
        //Debug.Log(selectedPrize);
        float startAngle = m_wheelContent.localEulerAngles.z;
        m_currentTime = 0.0f;

        float targetAngle = (m_numberCirclestoRotate * m_circleAngles) + m_angleOnePrize * prizeIDX - startAngle;

        while (m_currentTime <= m_rotateTime)
        {
            m_currentTime += Time.fixedDeltaTime;
            float currAngle = targetAngle * m_curve.Evaluate(m_currentTime / m_rotateTime);
            m_wheelContent.localEulerAngles = new Vector3(0.0f, 0.0f, currAngle + startAngle);
            m_wheelBackground.localEulerAngles = new Vector3(0.0f, 0.0f, currAngle + startAngle);

            yield return new WaitForFixedUpdate();
        }

        TanksMP.AudioManager.Play2D(m_spinEndedSFX);
        m_endSpinTween.PunchEffect();
        m_winningParticlePrefab.Stop();
        m_winningParticlePrefab.Play();
        OnEnergyUpdated();
        m_spinning = false;

        GivePrize(m_prizes[prizeIDX]);
        m_storeScreen.LockButtons(false);
    }

    public void ShuffleRewards()
    {
        m_prizes = m_prizes.ShuffleList();
        DisplayRewards();
    }

    public async void GivePrize(GPWheelPrize prize)
    {
        switch (prize.m_desc.m_type)
        {
            case GP_PRIZE_TYPE.kGold:
                m_LoadIndicator.SetActive(true);
                await APIManager.Instance.PlayerData.AddCoins(prize.m_prizeAmount);
                m_LoadIndicator.SetActive(false);
                break;
            case GP_PRIZE_TYPE.kGems:
                m_LoadIndicator.SetActive(true);
                await APIManager.Instance.PlayerData.AddGems(prize.m_prizeAmount);
                m_LoadIndicator.SetActive(false);
                break;
            case GP_PRIZE_TYPE.kEnergy:
                GPPlayerProfile.m_instance.AddEnergy(prize.m_prizeAmount);
                break;
            case GP_PRIZE_TYPE.kWoodenChest:
                {
                    List<GPStoreChestSO> chests = new List<GPStoreChestSO>();
                    for (int i = 0; i < prize.m_prizeAmount; i++)
                    {
                        chests.Add(m_woodChest);
                    }
                    m_chestOpeningLogic.OpenChestsInSequence(chests);
                    break;
                }
            case GP_PRIZE_TYPE.kGoldenChest:
                {
                    List<GPStoreChestSO> chests = new List<GPStoreChestSO>();
                    for (int i = 0; i < prize.m_prizeAmount; i++)
                    {
                        chests.Add(m_goldenChest);
                    }
                    m_chestOpeningLogic.OpenChestsInSequence(chests);
                    break;
                }
            default:
                break;
        }
    }

    void OnEnergyUpdated()
    {
        m_energyAmountText.text = GPPlayerProfile.m_instance.m_energy.ToString();
        m_fillTargetValue = (float)GPPlayerProfile.m_instance.m_energy / (float)GPPlayerProfile.m_instance.m_maxEnergy;
        if (GPPlayerProfile.m_instance.m_energy < m_spinWheelCost)
        {
            m_spinButton.interactable = false;
        }
        else
        {
            m_spinButton.interactable = true;
        }
    }

}
