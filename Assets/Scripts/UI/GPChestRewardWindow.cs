using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GPChestRewardWindow : GPGWindowUI
{
    [Header("UI references")]
    public Image m_chestImage;
    public Image m_chestTierTextImage;
    public TextMeshProUGUI m_upperRewardLabel;
    public TextMeshProUGUI m_lowerRewardLabel;

    public GameObject m_dummyPartDisplayerPrefab;
    public GameObject m_crewDisplayerPrefab;
    public GameObject m_profileIconDisplayerPrefab;

    public Transform m_dummyPartsHolder;
    public Transform m_crewsHolder;
    public Transform m_profileIconsHolder;

    [Header("Other settings")]
    public Vector3 m_crewCardScaleOverride;
    public GPPunchTween m_punchTween;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Show()
    {
        base.Show();
        m_punchTween.PunchEffect();
    }

    /// <summary>
    /// Clears the old reward UI blocks.
    /// </summary>
    public void ClearContent()
    {
        foreach (Transform child in m_dummyPartsHolder)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (Transform child in m_crewsHolder)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (Transform child in m_profileIconsHolder)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Displays the sprite of the openned chest based on the chest type.
    /// </summary>
    /// <param name="chestDesc"></param>
    public void DisplayChestImage(GPStoreChestSO chestDesc)
    {
        m_chestImage.sprite = chestDesc.m_chestRewardIcon;
        m_chestTierTextImage.sprite = chestDesc.m_chestRewardText;
    }

    /// <summary>
    /// Display the given dummy parts as rewards.
    /// </summary>
    /// <param name="dummyParts"></param>
    public void DisplayDummyRewards(List<GPDummyPartDesc> dummyParts)
    {
        for (int i = 0; i < dummyParts.Count; i++)
        {
            GameObject displayer = Instantiate(m_dummyPartDisplayerPrefab, m_dummyPartsHolder);
            displayer.GetComponent<GPDummyPartBlock>().DisplayPart(dummyParts[i]);
        }
    }

    /// <summary>
    /// Display the give crews as rewards.
    /// </summary>
    /// <param name="ships"></param>
    public void DisplayCrewRewards(List<GPShipDesc> ships)
    {
        for (int i = 0; i < ships.Count; i++)
        {
            GameObject displayer = Instantiate(m_crewDisplayerPrefab, m_crewsHolder);
            displayer.GetComponent<GPShipCard>().DisplayShipDesc(ships[i]);
            displayer.transform.localScale = m_crewCardScaleOverride;
        }
    }

    /// <summary>
    /// Display the given profile icon as rewards.
    /// </summary>
    /// <param name="icons"></param>
    public void DisplayIconRewards(List<GPProfileIconSO> icons)
    {
        for (int i = 0; i < icons.Count; i++)
        {
            GameObject displayer = Instantiate(m_profileIconDisplayerPrefab, m_profileIconsHolder);
            displayer.GetComponent<GPProfileIconBlock>().SetProfileIconDesc(icons[i]);
        }
    }
}
