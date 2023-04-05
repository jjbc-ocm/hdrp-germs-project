
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HomeUI : WindowUI<HomeUI>
{
    [SerializeField]
    private Transform cameraPos;

    async void Start()
    {
        SpinnerUI.Instance.Open();

        await APIManager.Instance.PlayerData.Get();

        SpinnerUI.Instance.Close();
    }

    public void OnPlayClick()
    {
        MatchMakingUI.Instance.Open();

        MenuNetworkManager.Instance.TryStartMatchMaking();
    }

    public void OnCrewClick()
    {
        CrewUI.Instance.Open((self) => { });

        Close();
    }

    public void OnDummyClick()
    {
        DummyUI.Instance.Open();

        Close();
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
        Camera.main.transform.DOMove(cameraPos.transform.position, 0.25f).SetEase(Ease.InOutBounce);
    }
}