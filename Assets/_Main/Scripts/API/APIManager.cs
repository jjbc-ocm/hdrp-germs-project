using Photon.Pun;
using Steamworks;
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

public class APIManager : Singleton<APIManager>
{
    [SerializeField]
    private GPDevFeaturesSettingsSO devSettings;

    [SerializeField]
    private GPShipDesc startingShip;

    private PlayerData playerData;

    public PlayerData PlayerData { get => playerData; }


    #region Unity

    private void Awake()
    {
        LoadingUI.Instance.RefreshUI();

        Initialize((text, value) =>
        {
            LoadingUI.Instance.RefreshUI((self) =>
            {
                self.Text = text;

                self.Progress = value;
            });
        });
    }

    private void OnApplicationQuit()
    {
        SteamClient.Shutdown();
    }

    #endregion




    #region Public

    public void Initialize(Action<string, float> onProgress)
    {
        try
        {
            SteamClient.Init(SOManager.Instance.Constants.AppID);
        }
        catch (Exception e)
        {
            Debug.LogError(e);

            // TODO: notify the player to open their steam or connect to internet to be able to continue

            if (!devSettings.LoginAsAnonymous) return;
        }

        
        onProgress.Invoke("Logging in with Steam...", 0);

        /* Auto-login via steam */
        LogInWithSteam(async (sessionTicket) =>
        {
            var options = new InitializationOptions();

            options.SetEnvironmentName(devSettings.Environment);

            await UnityServices.InitializeAsync(options);

            AuthenticationService.Instance.SignOut();

            await LogInWithUnity(onProgress, devSettings.LoginAsAnonymous ? null : sessionTicket);
        });
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

    #endregion

    #region Private

    private void LogInWithSteam(Action<string> onSuccess)
    {
        if (!devSettings.LoginAsAnonymous)
        {
            var ticket = SteamUser.GetAuthSessionTicket();

            onSuccess.Invoke(BitConverter.ToString(ticket.Data).Replace("-", string.Empty));
        }
        else
        {
            onSuccess.Invoke(null);
        }
    }

    private async Task LogInWithUnity(Action<string, float> onProgress, string steamSessionTicket = null)
    {
        try
        {
            if (steamSessionTicket == null)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            else
            {
                await AuthenticationService.Instance.SignInWithSteamAsync(steamSessionTicket);
            }
        }
        catch (Exception)
        {
            onProgress.Invoke("Login failed... Logging in as guest instead...", 0.25f);

            await LogInWithUnity(onProgress, null);

            return;
        }
        

        /* Get player data */
        onProgress.Invoke("Fetching player data...", 0.5f);

        playerData = await new PlayerData().Get();

        /* Initialize level, exp, and starting ship if first time playing the game */
        if (!playerData.IsInitialized)
        {
            playerData.SetLevel(1).SetExp(0).SetInitialized(true).SetSelectedShipID(startingShip.ID);
        }

        /* Always update the name based on the name using on Steam */
        playerData.SetName(devSettings.LoginAsAnonymous ? "Anonymous User" : SteamClient.Name);

        /* Save data to cloud save */
        await playerData.Put();

        /* Load next scene */
        onProgress.Invoke("Loading game...", 0.9f);

        SceneManager.LoadScene(SOManager.Instance.Constants.SceneMenu);
    }

    #endregion
}
