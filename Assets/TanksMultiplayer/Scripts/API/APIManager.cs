using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

public class APIManager : MonoBehaviour
{
    public static APIManager Instance;

    void Awake()
    {
        Instance = this;

        Debug.Log(bool.Parse("true"));
        Debug.Log(bool.Parse("false"));
        Debug.Log(bool.Parse(null));
        Debug.Log(bool.Parse(""));
        Debug.Log(bool.Parse("DFGSGF"));

        Initialize();

        DontDestroyOnLoad(gameObject);
    }






    private async void Initialize()
    {
        var options = new InitializationOptions();

        options.SetEnvironmentName(Constants.ENV_NAME);

        await UnityServices.InitializeAsync(options);

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        // if first login, initialize level = 1, exp = 0, and isFirstLogin = false
        // else, load data

        var data = await CloudSaveService.Instance.Data.LoadAllAsync();

        Debug.Log(data);

        /*if (data["isFirstLogin"])
        {

        }
        else
        {

        }

        var data = new Dictionary<string, object>
        {
            { "level", 1 },
            { "exp", 0 }
        };

        await CloudSaveService.Instance.Data.ForceSaveAsync(data);*/
    }
}
