using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            return GetInstance();
        }
    }

    private void Awake()
    {
        GetInstance();
    }

    private static T GetInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<T>(true);
        }

        return instance;
    }
}
