using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TanksMP;
using Photon.Pun;

public class GPProfileWindow : GPGWindowUI
{
    public Transform m_iconsHolder;
    public GPProfileIconBlock m_profileIconBlockPrefab;
    List<GPProfileIconBlock> m_spawnedBlocks = new List<GPProfileIconBlock>();

    public GPProfileIconBlock m_lastPreviewedBlock;
    public GPUserFrameUI m_profileWindowUserMiniature;
    public GPUserFrameUI m_homeUserMiniature;

    [Header("Bio")]
    public TextMeshProUGUI m_nameText;
    public TextMeshProUGUI m_winsText;
    public TextMeshProUGUI m_lossText;
    public TextMeshProUGUI m_drawText;
    public TextMeshProUGUI m_KDRatioText;
    public TextMeshProUGUI m_winRateText;

    [Header("Exp")]
    public Image m_expFill;
    public TextMeshProUGUI m_expText;

    [Header("Audio settings")]
    public AudioClip m_equipSFX;

    // Start is called before the first frame update
    void Start()
    {
        m_closeButton.onClick.AddListener(Hide);

        //instantiate the profile icon UI blocks for all existing icons
        for (int i = 0; i < GPItemsDB.m_instance.m_profileIcons.Count; i++)
        {
            GPProfileIconBlock block = Instantiate(m_profileIconBlockPrefab, m_iconsHolder);
            block.SetProfileIconDesc(GPItemsDB.m_instance.m_profileIcons[i]);
            block.onClickedEvent.AddListener(OnClickedIcon);
            m_spawnedBlocks.Add(block);
        }

        //Display the name from the API
        m_nameText.text = APIManager.Instance.PlayerData.Name;

        //Display the current level text and EXP amount in the UI
        UpdateLevelText();
        UpdateExpUI();
    }

    void Update()
    {
        WriteStats(APIManager.Instance.PlayerData.Stats);
    }

    public override void Show()
    {
        base.Show();
        TanksMP.AudioManager.Play2D(m_showSound);

        foreach (var block in m_spawnedBlocks)
        {
            //Display as locked or unlocked
            block.ToggleLocked(!GPPlayerProfile.m_instance.m_profileIcons.Contains(block.m_profileIconDesc));

            if (block.m_profileIconDesc.m_sprite == m_homeUserMiniature.m_assignedProfileIconSprite)
            {
                OnClickedIcon(block);
            }
        }

        //Display the current level text and EXP amount in the UI
        UpdateLevelText();
        UpdateExpUI();
    }

    public override void Hide()
    {
        base.Hide();
        TanksMP.AudioManager.Play2D(m_hideSound);
    }

    /// <summary>
    /// If the clicked icon is unlocked then it is displayed on the preview profile icon.
    /// </summary>
    /// <param name="block"></param>
    void OnClickedIcon(GPProfileIconBlock block)
    {
        if (block.m_isLocked)
        {
            return;
        }

        m_lastPreviewedBlock = block;

        m_profileWindowUserMiniature.SetProfileIcon(block.m_profileIconDesc.m_sprite);
    }

    /// <summary>
    /// Binded to the "Equip" button.
    /// </summary>
    public void EquipProfileIcon()
    {
        if (m_lastPreviewedBlock)
        {
            m_homeUserMiniature.SetProfileIcon(m_lastPreviewedBlock.m_profileIconDesc.m_sprite);
        }
        TanksMP.AudioManager.Play2D(m_equipSFX);
    }

    /// <summary>
    /// Displays the player stats (wins, loss, draws, etc) on the profile window UI.
    /// </summary>
    /// <param name="data"></param>
    public void WriteStats(StatsData data)
    {
        m_winsText.text = data.Wins.ToString();
        m_lossText.text = data.Losses.ToString();
        m_drawText.text = data.Draws.ToString();

        var killsDeaths = data.Kills + data.Deaths;

        int KDRatio = killsDeaths > 0 ? Mathf.RoundToInt(data.Kills / (float)killsDeaths * 100.0f) : 0;

        m_KDRatioText.text = KDRatio.ToString() + "%";

        var winsLosses = data.Wins + data.Losses;

        int winRate = winsLosses > 0 ? Mathf.RoundToInt(data.Wins / winsLosses * 100.0f) : 0;

        m_winRateText.text = winRate.ToString() + "%";
    }

    /// <summary>
    /// Upates the displayed player level.
    /// </summary>
    void UpdateLevelText()
    {
        m_homeUserMiniature.SetLevel(APIManager.Instance.PlayerData.Level);
        m_profileWindowUserMiniature.SetLevel(APIManager.Instance.PlayerData.Level);
    }

    /// <summary>
    /// Upates the displayed player EXP on the fill bar and text.
    /// </summary>
    void UpdateExpUI()
    {
        int nextLevelEXP = 100; // TODO: Calcualte this from another system.
        m_expFill.fillAmount = (float)APIManager.Instance.PlayerData.Exp / (float)nextLevelEXP;
        m_expText.text = APIManager.Instance.PlayerData.Exp.ToString() + "/" + nextLevelEXP.ToString() + " XP";
    }

}
