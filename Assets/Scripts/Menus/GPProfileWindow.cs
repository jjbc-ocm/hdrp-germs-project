using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TanksMP;

public class GPProfileWindow : GPGWindowUI
{
    public Transform m_iconsHolder;
    public GPProfileIconBlock m_profileIconBlockPrefab;
    List<GPProfileIconBlock> m_spawnedBlocks = new List<GPProfileIconBlock>();

    public List<Image> m_displayedImages; // it's a list because of the diferent level frames
    public List<Image> m_mainMenuProfileImages; // it's a list because of the diferent level frames
    public GPProfileIconBlock m_lastPreviewedBlock;

    [Header("Bio")]
    public TextMeshProUGUI m_nameText;
    public TextMeshProUGUI m_winsText;
    public TextMeshProUGUI m_lossText;
    public TextMeshProUGUI m_drawText;
    public TextMeshProUGUI m_KDRatioText;
    public TextMeshProUGUI m_winRateText;

    // Start is called before the first frame update
    void Start()
    {
        m_closeButton.onClick.AddListener(Hide);

        for (int i = 0; i < GPItemsDB.m_instance.m_profileIcons.Count; i++)
        {
            GPProfileIconBlock block = Instantiate(m_profileIconBlockPrefab, m_iconsHolder);
            block.SetProfileIconDesc(GPItemsDB.m_instance.m_profileIcons[i]);
            block.onClickedEvent.AddListener(OnClickedIcon);
            m_spawnedBlocks.Add(block);
        }

        // set up stats
        if (PlayerPrefs.HasKey(PrefsKeys.playerName))
        {
            m_nameText.text = PlayerPrefs.GetString(PrefsKeys.playerName);
        }
    }

    public override void Show()
    {
        base.Show();
        TanksMP.AudioManager.Play2D(m_showSound);

        foreach (var block in m_spawnedBlocks)
        {
            block.ToggleLocked(!GPPlayerProfile.m_instance.m_profileIcons.Contains(block.m_profileIconDesc));
        }
    }

    public override void Hide()
    {
        base.Hide();
        TanksMP.AudioManager.Play2D(m_hideSound);
    }

    void OnClickedIcon(GPProfileIconBlock block)
    {
        if (block.m_isLocked)
        {
            return;
        }

        m_lastPreviewedBlock = block;
        foreach (var image in m_displayedImages)
        {
            image.sprite = block.m_profileIconDesc.m_sprite;
        }
    }

    /// <summary>
    /// Binded to the "Equip" button.
    /// </summary>
    public void EquipProfileIcon()
    {
        if (m_lastPreviewedBlock)
        {
            foreach (var image in m_mainMenuProfileImages)
            {
                image.sprite = m_lastPreviewedBlock.m_profileIconDesc.m_sprite;
            }
        }
    }

    public void WriteStats(StatsData data)
    {
        m_winsText.text = data.Wins.ToString();
        m_lossText.text = data.Losses.ToString();
        m_drawText.text = data.Draws.ToString();

        int KDRatio = Mathf.RoundToInt(data.Kills / (float)(data.Kills + data.Deaths) * 100.0f);
        m_KDRatioText.text = KDRatio.ToString() + "%";

        int winRate = Mathf.RoundToInt(data.Wins / (float)(data.Wins + data.Losses) * 100.0f);
        m_winRateText.text = winRate.ToString() + "%";
    }

}
