using System.Collections;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using System.Runtime.InteropServices;
using Nethereum.Util;

public class GPGConectToWalletMenu : MonoBehaviour
{
  public string m_nextSceneName = "Intro";
  [TextArea(7, 10)]
  public string m_authMsg = "Welcome to Germ Pirates.\nThis request is only to verify your address with us and this will not trigger a blockchain transaction.";

  [DllImport("__Internal")]
  private static extern void Web3Connect();

  [DllImport("__Internal")]
  private static extern string ConnectAccount();

  [DllImport("__Internal")]
  private static extern void SetConnectAccount(string value);

  private string account;

  bool m_walletConnected = false;
  bool m_loadSceneCalled = false;
  bool m_messageSigned = false;

  bool m_signing = false;

  private void Start()
  {
  }

  private void Update()
  {
    if (m_walletConnected && GermAPIConnector.m_instance.m_connected && !m_loadSceneCalled && m_messageSigned)
    {
      SceneManager.LoadScene(m_nextSceneName);
      m_loadSceneCalled = true;
    }
  }

  public void OnConnectButtonPressed()
  {
    if (m_messageSigned)
    {
      return;
    }
#if UNITY_WEBGL
  #if !UNITY_EDITOR
    Web3Connect();
  #endif
    StartCoroutine(LoginWebGL());
#else // desktop or mobile
    StartCoroutine(LoginDesktopAndMobile());
#endif

  }

  IEnumerator LoginWebGL()
  {
    // if playing in editor then skip metamask an use a fixed address for quick testing reasons.
#if UNITY_WEBGL && !UNITY_EDITOR
    account = ConnectAccount();
    while (account == "")
    {
      //await new WaitForSeconds(1f);
      yield return new WaitForSeconds(1.0f);
      account = ConnectAccount();
    };
#else
#if UNITY_EDITOR
    account = "0x0000000000000000000000000000000000000000"; // use test address while in unity editor (only if WEBGL is activated)
#else
    account = "0x0000000000000000000000000000000000000000";
#endif
    yield return new WaitForSeconds(1.0f);
#endif
    if (!AddressExtensions.IsEthereumChecksumAddress(account))
    {
      account = AddressExtensions.ConvertToEthereumChecksumAddress(account);
    }
    // save account for next scene
    PlayerPrefs.SetString("Account", account);

#if UNITY_WEBGL && !UNITY_EDITOR
    // reset login message
    SetConnectAccount("");
#endif
    m_walletConnected = true;
#if CD_LOGS
    Debug.Log("Account:" + account);
#endif

    yield return StartCoroutine(GermAPIConnector.m_instance.IERequestAllData(account));

    /*
    Task<bool> task = CheckEntryRequiriments();
    yield return new WaitUntil(() => task.IsCompleted);
    bool canEnter = task.Result;
    if (canEnter == false)
    {
      CDMessageManager.m_instance.ShowMessage("ACCESS DENIED.\n\nThis address must have a NFT of any of the following types:", CD_MESSAGE_TYPE.kWarning);
      yield break;
    }
    */

    if (!m_signing && !m_messageSigned)
    {
      string Hash = GPGAPIConnector.HashString(DateTime.UtcNow.ToString());
      string message = m_authMsg + "\n\nHash: " + Hash;
      AskSignMessage(message);
    }
  }

  public IEnumerator LoginDesktopAndMobile()
  {
    // get current timestamp
    int timestamp = (int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
    // set expiration time
    int expirationTime = timestamp + 60;

    if (!m_messageSigned)
    {
      string Hash = GPGAPIConnector.HashString(DateTime.UtcNow.ToString());
      string message = m_authMsg + "\n\nHash: " + Hash;

      Task<string> signTask = AskSignMessage(message);
      yield return new WaitUntil(() => signTask.IsCompleted);
      string signature = signTask.Result;

      // verify account
      Task<string> verifyTask = EVM.Verify(message, signature);
      yield return new WaitUntil(() => verifyTask.IsCompleted);
      account = verifyTask.Result;

      int now = (int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
      // validate
      if (account.Length == 42 && expirationTime >= now)
      {
        if (!AddressExtensions.IsEthereumChecksumAddress(account))
        {
          account = AddressExtensions.ConvertToEthereumChecksumAddress(account);
        }

        // save account
        PlayerPrefs.SetString("Account", account);
        print("Account: " + account);
        m_walletConnected = true;
      }
    }

    yield return StartCoroutine(GermAPIConnector.m_instance.IERequestAllData(account));
  }

  async Task<string> AskSignMessage(string msg)
  {
    m_signing = true;
    string resultSiganture = "";

    if (GermAPIConnector.m_useAPI)
    {
//#if UNITY_WEBGL && !UNITY_EDITOR
      GPGStatusMessage authMessage = (GPGStatusMessage)GPGMessageManager.m_instance.ShowSignMessage(msg, "AUTHENTICATION");
      authMessage.SetPendingState();
      string result = await GPGCryptoManager.m_instance.SignMessage(msg);
      if (result != "Error" && result != "")
      {
        authMessage.SetCompletedState();
      }
      else
      {
        authMessage.SetCancelledState();
      }
      await new WaitForSeconds(1.0f);
      authMessage.Hide();
      if (result != "Error" && result != "")
      {
        string signature = result;
        Debug.Log("Signature: " + signature);

        resultSiganture = signature;
        GermAPIConnector.m_registrySignature = signature;
        GermAPIConnector.m_registryMessage = msg;

        m_messageSigned = true;
      }
//#else
     //m_messageSigned = true;
//#endif
    }
    else
    {
      m_messageSigned = true;
    }

    m_signing = false;
    return resultSiganture;
  }

  public async Task<bool> CheckEntryRequiriments()
  {
    //Check for NFTs ownership here
    return true;
  }
}
