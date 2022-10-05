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

[Serializable]
public class GPGAPIResponse
{
  public string result;
  public string error;
  public UnityWebRequest.Result webResult;

  public GPGAPIResponse()
  {
    result = "";
    error = "";
    webResult = UnityWebRequest.Result.ConnectionError;
  }

  public GPGAPIResponse(string _response, string _error, UnityWebRequest.Result _webResult)
  {
    result = _response;
    error = _error;
    webResult = _webResult;
  }
}

public class GPGAPIConnector : MonoBehaviour
{
  public virtual IEnumerator GetData(System.Action<GPGAPIResponse> callback, string uri)
  {
    UnityWebRequest www = UnityWebRequest.Get(uri);
    yield return www.SendWebRequest();

    if (www.result == UnityWebRequest.Result.ConnectionError)
    {
#if CD_LOGS
      Debug.Log(www.error);
#endif
    }
    else
    {
#if CD_LOGS
      Debug.Log(www.downloadHandler.text);
#endif
    }

    callback(new GPGAPIResponse(www.downloadHandler.text, www.error,www.result));
    www.Dispose();
  }

  public virtual IEnumerator PostJson(System.Action<GPGAPIResponse> callback, string uri, string json)
  {
    UnityWebRequest www = new UnityWebRequest(uri, "POST");
    byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
    www.uploadHandler = new UploadHandlerRaw(jsonToSend);
    www.downloadHandler = new DownloadHandlerBuffer();
    www.SetRequestHeader("Content-Type", "application/json");

    //Send the request then wait here until it returns
    yield return www.SendWebRequest();

    if (www.result == UnityWebRequest.Result.ConnectionError)
    {
#if CD_LOGS
      Debug.Log("Post Json error: " + www.error);
#endif
    }
    else
    {
#if CD_LOGS
      Debug.Log("Post Json result: " + www.downloadHandler.text);
#endif
    }

    callback(new GPGAPIResponse(www.downloadHandler.text, www.error, www.result));
    www.Dispose();
  }

  public static string HashString(string text, string salt = "")
  {
    if (String.IsNullOrEmpty(text))
    {
      return String.Empty;
    }

    // Uses SHA256 to create the hash
    using (var sha = new System.Security.Cryptography.SHA256Managed())
    {
      // Convert the string to a byte array first, to be processed
      byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text + salt);
      byte[] hashBytes = sha.ComputeHash(textBytes);

      // Convert back to a string, removing the '-' that BitConverter adds
      string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

      return hash;
    }
  }

}
