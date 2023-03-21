
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HomeUI : UI<HomeUI>
{
    async void Start()
    {
        SpinnerUI.Instance.Open();

        await APIManager.Instance.PlayerData.Get();

        SpinnerUI.Instance.Close();
    }

    public void OnPlayClick()
    {
        MatchMakingUI.Instance.Open();

        MenuNetworkManager.Instance.StartMatchMaking((text) =>
        {
            Debug.Log(text);
        });
    }

    public void OnSettingsClick()
    {
        SettingsUI.Instance.Open((self) =>
        {
            self.Data = new SettingsData(APIManager.Instance.PlayerData.Settings);
        });
    }

    public void OnExitClick()
    {
        Application.Quit();
    }

    protected override void OnRefreshUI()
    {

    }
}