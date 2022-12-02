using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.SceneManagement;

public class APIManager : MonoBehaviour
{
    public static APIManager Instance;

    void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        //Initialize();
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

        var data = await new PlayerData().Get();

        if (!data.IsInitialized)
        {
            data.SetLevel(1).SetExp(0).SetInitialized(true);

            await data.Put();

            await data.Get();
        }

        Debug.Log(data.Level + " " + data.Exp);

        /* Load next scene */
        onProgress.Invoke("Loading game...", 0.9f);

        SceneManager.LoadScene(Constants.MENU_SCENE_NAME);
    }
}
