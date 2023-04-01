using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPWaitingRoomDevFeatures : MonoBehaviour
{
    public GPDevFeaturesSettingsSO m_devSettings;
    public GameObject m_skipPlayerSearchDevCheat;
    public GameObject m_autoFillBotDevCheat;

    // Start is called before the first frame update
    void Start()
    {
        m_skipPlayerSearchDevCheat.SetActive(m_devSettings.m_skipPlayerSearch);

        m_autoFillBotDevCheat.SetActive(m_devSettings.m_autoFillBot);
    }
}
