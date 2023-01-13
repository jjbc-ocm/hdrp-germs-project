using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPMainMenuDevFeatures : MonoBehaviour
{
    public GPDevFeaturesSettingsSO m_devSettings;

    [Header("Features in development")]
    public GameObject m_storeButton;
    public GameObject m_expBar;
    public List<GameObject> m_friendListObjects;

    [Header("Dev Cheats")]
    public GameObject m_levelUpDevCheat;
    public GameObject m_weeklyRewardDevCheat;

    // Start is called before the first frame update
    void Start()
    {
        //Features
        m_storeButton.SetActive(m_devSettings.m_store);
        m_expBar.SetActive(m_devSettings.m_expBar);
        foreach (var obj in m_friendListObjects)
        {
            obj.SetActive(m_devSettings.m_friends);
        }

        //Dev cheats
        m_levelUpDevCheat.SetActive(m_devSettings.m_levelUpButton);
        m_weeklyRewardDevCheat.SetActive(m_devSettings.m_weeklyRewardButton);
    }
    
}
