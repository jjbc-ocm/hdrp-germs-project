using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Economy;
using UnityEngine;
using UnityEngine.SceneManagement;

public class APIManager : MonoBehaviour
{
    public static APIManager Instance;

    [SerializeField]
    private GPShipDesc startingShip;

    [SerializeField]
    private LoadingUI uiLoading;

    private PlayerData playerData;

    public PlayerData PlayerData { get => playerData; }

    void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        Initialize((text, value) =>
        {
            uiLoading.Text = text;

            uiLoading.Progress = value;
        });
    }






    public async void Initialize(Action<string, float> onProgress)
    {
        /* Auto-login */
        onProgress.Invoke("Logging in...", 0);

        var options = new InitializationOptions();

        options.SetEnvironmentName(Constants.ENV_NAME);

        await UnityServices.InitializeAsync(options);

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        /* Get player data */
        onProgress.Invoke("Fetching player data...", 0.5f);

        playerData = await new PlayerData().Get();

        if (!playerData.IsInitialized)
        {
            playerData.SetLevel(1).SetExp(0).SetInitialized(true).SetSelectedShipID(startingShip.ID);

            await playerData.Put();
        }

        /* Load next scene */
        onProgress.Invoke("Loading game...", 0.9f);

        SceneManager.LoadScene(Constants.MENU_SCENE_NAME);
    }

    public async Task<bool> TryVirtualPurchase(string id)
    {
        try
        {
            var result = await EconomyService.Instance.Purchases.MakeVirtualPurchaseAsync(id);

            await playerData.Get();

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
