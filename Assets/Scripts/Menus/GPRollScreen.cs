using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KaimiraGames;

public enum GP_PRIZE_TYPE
{
    kGold,
    kGems,
    kEnergy,
    kWoodenChest,
    kGoldenChest,
}

[System.Serializable]
public class GPWheelPrize
{
    public GP_PRIZE_TYPE m_prizeType;
    public int m_prizeAmount;
    public int m_weight;
}

public class GPRollScreen : GPGUIScreen
{
    [Header("UI references")]
    public Button m_spinButton;
    public Transform m_wheelContent;
    public Transform m_wheelBackground;

    [Header("Prize settings")]
    [Tooltip("List them in clock wise order starting from the top of the wheel")]
    public List<GPWheelPrize> m_prizes;
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

    [Header("Audio Settings")]
    public AudioClip m_spinStartedSFX;
    public AudioClip m_spinEndedSFX;

    [Header("Energy Settings")]
    public int m_spinWheelCost = 10;
    public Image m_energyFill;
    public float m_fillAnimSpeed = 7.0f;
    float m_fillTargetValue = 0.0f;

    [Header("Misc.")]

    [SerializeField]
    private GameObject m_LoadIndicator;

    void Start()
    {
        for (int i = 0; i < m_prizes.Count; i++)
        {
            m_weightedList.Add(m_prizes[i].m_prizeType.ToString() + " " + m_prizes[i].m_prizeAmount.ToString(),
                               m_prizes[i].m_weight);
        }

        m_angleOnePrize = m_circleAngles / m_prizes.Count;
        m_spinButton.onClick.AddListener(Spin);
        OnEnergyUpdated();

        GPPlayerProfile.m_instance.OnEnergyModifiedEvent.AddListener(OnEnergyUpdated);
    }

    private void Update()
    {
        m_energyFill.fillAmount = Mathf.Lerp(m_energyFill.fillAmount, m_fillTargetValue, Time.deltaTime * m_fillAnimSpeed);
    }

    public override void Show()
    {
        base.Show();
        OnEnergyUpdated();
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
        OnEnergyUpdated();
        m_spinning = false;

        GivePrize(m_prizes[prizeIDX]);
    }

    public async void GivePrize(GPWheelPrize prize)
    {
        switch (prize.m_prizeType)
        {
            case GP_PRIZE_TYPE.kGold:
                //GPPlayerProfile.m_instance.AddGold(prize.m_prizeAmount);
                m_LoadIndicator.SetActive(true);
                await APIManager.Instance.PlayerData.AddCoins(prize.m_prizeAmount);
                m_LoadIndicator.SetActive(false);
                break;
            case GP_PRIZE_TYPE.kGems:
                //GPPlayerProfile.m_instance.AddGems(prize.m_prizeAmount);
                m_LoadIndicator.SetActive(true);
                await APIManager.Instance.PlayerData.AddGems(prize.m_prizeAmount);
                m_LoadIndicator.SetActive(false);
                break;
            case GP_PRIZE_TYPE.kEnergy:
                GPPlayerProfile.m_instance.AddEnergy(prize.m_prizeAmount);
                break;
            case GP_PRIZE_TYPE.kWoodenChest:
                for (int i = 0; i < prize.m_prizeAmount; i++)
                {
                    m_woodChest.OpenChest();
                }
                break;
            case GP_PRIZE_TYPE.kGoldenChest:
                for (int i = 0; i < prize.m_prizeAmount; i++)
                {
                    m_goldenChest.OpenChest();
                }
                break;
            default:
                break;
        }
    }

    void OnEnergyUpdated()
    {
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
