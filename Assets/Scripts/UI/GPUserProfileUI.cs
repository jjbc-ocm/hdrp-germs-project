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
        //Set the displayed name to be the the one of the APIManager player data.
        m_inputField.text = APIManager.Instance.PlayerData.Name;
    }

    /// <summary>
    /// Updates the displayed name using the current text of the input field.
    /// </summary>
    public void UpdateName()
    {
        PlayerPrefs.SetString(PrefsKeys.playerName, m_inputField.text);
    }

}
