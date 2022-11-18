using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TanksMP;

public class GPUserProfileUI : MonoBehaviour
{
    public TMP_InputField m_inputField;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey(PrefsKeys.playerName))
        {
            m_inputField.text = PlayerPrefs.GetString(PrefsKeys.playerName);
        }
    }

    public void UpdateName()
    {
        PlayerPrefs.SetString(PrefsKeys.playerName, m_inputField.text);
    }
    
}
