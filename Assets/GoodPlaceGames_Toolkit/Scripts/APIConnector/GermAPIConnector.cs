using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class GermAPIConnector : GPGAPIConnector
{
  public static GermAPIConnector m_instance;

  [Tooltip("If true this component will persist across all scenes.")]
  public bool m_dontDestroyOnLoad = true;

  public bool m_connected = false;

  //Stored Data
  public static string m_registrySignature;
  public static string m_registryMessage;

  //debug
  public static bool m_useAPI = true;

  private void Awake()
  {
    if (m_instance == null)
    {
      m_instance = this;
      if (m_dontDestroyOnLoad)
      {
        DontDestroyOnLoad(m_instance);
      }
    }
    else
    {
      Destroy(gameObject);
      return;
    }

#if UNITY_EDITOR
    //m_useAPI = false;
#endif
  }

  private void Start()
  {

  }

  private void FixedUpdate()
  {
  }

  public void RequestAllData(string address)
  {
    StartCoroutine(IERequestAllData(address));
  }

  public IEnumerator IERequestAllData(string address)
  {
    yield return 0;

    m_connected = true;
  }

  public override IEnumerator GetData(System.Action<GPGAPIResponse> callback, string uri)
  {
    GPGAPIResponse response = new GPGAPIResponse();
    yield return StartCoroutine(base.GetData((myReturnValue) => { response = myReturnValue; }, uri));

    if (response.webResult == UnityWebRequest.Result.ConnectionError)
    {
      //CDMessageManager.m_instance.ShowMessage("GetData error: " + response.error);
    }

    callback(response);
  }

  public override IEnumerator PostJson(System.Action<GPGAPIResponse> callback, string uri, string json)
  {
    GPGAPIResponse response =  new GPGAPIResponse();
    yield return StartCoroutine(base.PostJson((myReturnValue) => { response = myReturnValue; }, uri, json));

    if (response.webResult == UnityWebRequest.Result.ConnectionError)
    {
      //CDMessageManager.m_instance.ShowMessage("Post Json error: " + response.error);
    }

    callback(response);
  }

}
