using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GPG_NFT_DESC
{
  public string key;
  public string chain;
  public string network;
  public string contract;
  public string tokenID;
}

[CreateAssetMenu(fileName = "GPGNFTCollectionSO", menuName = "ScriptableObjects/GPGNFTCollectionSO")]
public class GPGNFTCollectionSO : ScriptableObject
{
  [SerializeField]
  protected List<GPG_NFT_DESC> m_projectNFTs;
  [HideInInspector]
  public Dictionary<string, GPG_NFT_DESC> m_NFTsMap = new Dictionary<string, GPG_NFT_DESC>();
  public void Init()
  {
    foreach (var nft in m_projectNFTs)
    {
      if (!m_NFTsMap.ContainsKey(nft.key))
      {
        m_NFTsMap.Add(nft.key, nft);
      }
    }
  }
}
