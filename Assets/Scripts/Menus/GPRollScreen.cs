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
    public float m_currEnergy = 0;
    public float m_maxEnergy = 10;
    public float m_spinWheelCost = 10;
    public Image m_energyFill;
    public float m_fillAnimSpeed = 7.0f;
    float m_fillTargetValue = 0.0f;

    void Start()
    {
        for (int i = 0; i < m_prizes.Count; i++)
        {
            m_weightedList.Add(m_prizes[i].m_prizeType.ToString() + " " + m_prizes[i].m_prizeAmount.ToString(),
                               m_prizes[i].m_weight);
        }

        m_angleOnePrize = m_circleAngles / m_prizes.Count;
        m_spinButton.onClick.AddListener(Spin);
        UpdateEnergy();
    }

    private void Update()
    {
        m_energyFill.fillAmount = Mathf.Lerp(m_energyFill.fillAmount, m_fillTargetValue, Time.deltaTime * m_fillAnimSpeed);
    }

    public override void Show()
    {
        base.Show();
        UpdateEnergy();
    }

    public void Spin()
    {
        if (!m_spinning && m_currEnergy >= m_spinWheelCost)
        {
            SpendEnergy(m_spinWheelCost);
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
        UpdateEnergy();
        m_spinning = false;
    }

    public void SpendEnergy(float amount)
    {
        m_currEnergy -= amount;
        UpdateEnergy();
    }

    void UpdateEnergy()
    {
        m_currEnergy = Mathf.Clamp(m_currEnergy, 0.0f, m_maxEnergy);
        m_fillTargetValue = m_currEnergy / m_maxEnergy;
        if (m_currEnergy < m_spinWheelCost)
        {
            m_spinButton.interactable = false;
        }
        else
        {
            m_spinButton.interactable = true;
        }
    }
}
