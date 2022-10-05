using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct GPG_CRYPTO_TOKEN_DESC
{
  public string symbol;
  public string contract;
  public Sprite sprite;
  public bool implemented;
}

[CreateAssetMenu(fileName = "GPGCryptoProjectSO", menuName = "ScriptableObjects/GPGCryptoProjectSO")]
public class GPGCryptoProjectSO : ScriptableObject
{
  public string m_chain = "polygon";
  public string m_network = "testnet";
  public string m_feeCollectorWallet = ""; // the wallet the entry fees tokens are going to.

  [SerializeField]
  protected List<GPG_CRYPTO_TOKEN_DESC> m_projectTokens;
  [HideInInspector]
  public Dictionary<string, GPG_CRYPTO_TOKEN_DESC> m_tokensMap = new Dictionary<string, GPG_CRYPTO_TOKEN_DESC>();

  public void Init()
  {
    foreach (var token in m_projectTokens)
    {
      if (!m_tokensMap.ContainsKey(token.symbol))
      {
        m_tokensMap.Add(token.symbol, token);
      }
    }
  }

}
