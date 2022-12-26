/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TanksMP
{
    /// <summary>
    /// UI script for all elements, settings and user interactions in the menu scene.
    /// </summary>
    public class UIMain : MonoBehaviour
    {
        [SerializeField]
        private LoadingUI uiLoading;

        [SerializeField]
        private GameObject indicatorLoad;

        async void Start()
        {
            //set initial values for all settings
            /*if (!PlayerPrefs.HasKey(PrefsKeys.playerName)) PlayerPrefs.SetString(PrefsKeys.playerName, "User" + System.String.Format("{0:0000}", Random.Range(1, 9999)));
            if (!PlayerPrefs.HasKey(PrefsKeys.networkMode)) PlayerPrefs.SetInt(PrefsKeys.networkMode, 0);
            if (!PlayerPrefs.HasKey(PrefsKeys.gameMode)) PlayerPrefs.SetInt(PrefsKeys.gameMode, 0);
            if (!PlayerPrefs.HasKey(PrefsKeys.serverAddress)) PlayerPrefs.SetString(PrefsKeys.serverAddress, "127.0.0.1");
            if (!PlayerPrefs.HasKey(PrefsKeys.playMusic)) PlayerPrefs.SetString(PrefsKeys.playMusic, "true");
            if (!PlayerPrefs.HasKey(PrefsKeys.appVolume)) PlayerPrefs.SetFloat(PrefsKeys.appVolume, 1f);
            if (!PlayerPrefs.HasKey(PrefsKeys.activeTank)) PlayerPrefs.SetString(PrefsKeys.activeTank, Encryptor.Encrypt("0"));

            PlayerPrefs.Save();*/

            indicatorLoad.SetActive(true);

            await APIManager.Instance.PlayerData.Get();

            indicatorLoad.SetActive(false);
        }

        public void DebugPlay()
        {
            uiLoading.gameObject.SetActive(true);

            MenuNetworkManager.Instance.enabled = false;

            DebugMenuNetworkManager.Instance.Play((text) =>
            {
                uiLoading.Text = text;
            });
        }

        public void Play()
        {
            uiLoading.gameObject.SetActive(true);

            DebugMenuNetworkManager.Instance.enabled = false;

            MenuNetworkManager.Instance.Play((text, progress) =>
            {
                uiLoading.Text = text;

                uiLoading.Progress = progress;
            });
        }

        public void OpenSettings()
        {
            SettingsUI.Instance.Open((self) =>
            {
                self.Data = new SettingsData(APIManager.Instance.PlayerData.Settings);
            });
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}