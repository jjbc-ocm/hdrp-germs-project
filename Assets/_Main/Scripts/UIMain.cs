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
            indicatorLoad.SetActive(true);

            await APIManager.Instance.PlayerData.Get();

            indicatorLoad.SetActive(false);
        }

        /*public void DebugPlay()
        {
            uiLoading.gameObject.SetActive(true);

            MenuNetworkManager.Instance.enabled = false;

            DebugMenuNetworkManager.Instance.Play((text) =>
            {
                uiLoading.Text = text;
            });
        }*/

        public void Play()
        {
            uiLoading.gameObject.SetActive(true);

            //DebugMenuNetworkManager.Instance.enabled = false;

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