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






    public void Initialize(Action<string, float> onProgress)
    {
        if (!SteamManager.Initialized)
        {
            // TODO: notify the player to open their steam or connect to internet to be able to continue

            return;
        }

        /* Auto-login */
        onProgress.Invoke("Logging in with Steam...", 0);

        LogInWithSteam(async (sessionTicket) =>
        {
            var options = new InitializationOptions();

            options.SetEnvironmentName(Constants.ENV_NAME);

            await UnityServices.InitializeAsync(options);

            await AuthenticationService.Instance.SignInWithSteamAsync(sessionTicket);

            /* Get player data */
            onProgress.Invoke("Fetching player data...", 0.5f);

            playerData = await new PlayerData().Get();
            
            /* Initialize level, exp, and starting ship if first time playing the game */
            if (!playerData.IsInitialized)
            {
                playerData.SetLevel(1).SetExp(0).SetInitialized(true).SetSelectedShipID(startingShip.ID);
            }

            /* Always update the name based on the name using on Steam */
            playerData.SetName(SteamFriends.GetPersonaName());

            /* Save data to cloud save */
            await playerData.Put();

            /* Load next scene */
            onProgress.Invoke("Loading game...", 0.9f);

            SceneManager.LoadScene(Constants.MENU_SCENE_NAME);
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



    private void LogInWithSteam(Action<string> onSuccess)
    {
        var sessionTicket = "";

        Callback<GetAuthSessionTicketResponse_t>.Create((callback) =>
        {
            onSuccess.Invoke(sessionTicket);
        });

        var buffer = new byte[1024];

        SteamUser.GetAuthSessionTicket(buffer, buffer.Length, out var ticketSize);

        Array.Resize(ref buffer, (int)ticketSize);

        sessionTicket = BitConverter.ToString(buffer).Replace("-", string.Empty);
    }
}
