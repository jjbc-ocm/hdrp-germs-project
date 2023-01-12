using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TanksMP;
using Photon.Pun;

public class GPUserProfileUI : MonoBehaviour
{
    public TMP_InputField m_inputField;

    // Start is called before the first frame update
    void Start()
    {
        m_inputField.text = APIManager.Instance.PlayerData.Name;
    }

    public void UpdateName()
    {
        PlayerPrefs.SetString(PrefsKeys.playerName, m_inputField.text);
    }

}
