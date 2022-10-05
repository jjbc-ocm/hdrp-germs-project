using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Linq;

public struct GPGTransaction
{
  public string transactionID;
  public string status;

  public GPGTransaction(string _id, string _status)
  {
    transactionID = _id;
    status = _status;
  }
}

public class GPGCryptoManager : MonoBehaviour
{
  public static GPGCryptoManager m_instance;
  public GPGCryptoProjectSO m_cryptoProject;
  public GPGNFTCollectionSO m_projectNFTs;
  public int m_transactionsToConfirmByUser = 0;
  public Dictionary<string, GPGTransaction> m_pendingTransactions = new Dictionary< string, GPGTransaction>();
  public Dictionary<string, GPGTransaction> m_confirmedTransactions = new Dictionary< string, GPGTransaction>();

  private float NextUpdateTime = 0.0f; // in seconds
  private float UpdateStatusInterval = 1.0f; // in seconds

  public UnityEvent<GPGTransaction> OnTransactionStatusUpdated;
  public UnityEvent<GPGTransaction> OnTransactionCompleted;
  public UnityEvent<GPGTransaction> OnTransactionCanceled;

  private void Awake()
  {
    if (m_instance == null)
    {
      m_instance = this;
      DontDestroyOnLoad(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }

    m_cryptoProject.Init();
    m_projectNFTs.Init();
  }

  private async void Start()
  {
  }

  private void Update()
  {
    // If the next update is reached
    if (Time.time >= NextUpdateTime)
    {
      NextUpdateTime = Time.time + UpdateStatusInterval;
      UpdateStatus();
    }
  }

  
  private async void UpdateStatus()
  {
    //Debug.Log("---UpdateStatus" + "Pending transactions: " + m_pendingTransactions.Count);
    foreach (var transaction in m_pendingTransactions.ToList())
    {
      string status = await GetTransactionStatus(transaction.Value.transactionID);

      GPGTransaction newValue = transaction.Value;
      newValue.status = status;
      m_pendingTransactions[transaction.Key] = newValue;

#if CD_LOGS
      Debug.Log("ID: " + m_pendingTransactions[transaction.Key].transactionID + " | status: " + m_pendingTransactions[transaction.Key].status);
#endif

      if (OnTransactionStatusUpdated != null)
      {
        OnTransactionStatusUpdated.Invoke(m_pendingTransactions[transaction.Key]);
      }

      if (status == "success" && OnTransactionCompleted != null)
      {
        OnTransactionCompleted.Invoke(m_pendingTransactions[transaction.Key]);
      }

      if (status == "fail" && OnTransactionCanceled != null)
      {
        OnTransactionCanceled.Invoke(m_pendingTransactions[transaction.Key]);
      }

      if (status == "fail")
      {
        m_pendingTransactions.Remove(transaction.Key);
        break;
      }

      if (status == "success")
      {
        m_confirmedTransactions.Add(transaction.Key, m_pendingTransactions[transaction.Key]);
        m_pendingTransactions.Remove(transaction.Key);
        break;
      }
    }
  }

  public void SetCryptoProyect(GPGCryptoProjectSO cryptoProject)
  {
    m_cryptoProject = cryptoProject;
    m_cryptoProject.Init();
  }

  public static async Task<double> GenericGetBalanceOf(string chain, string network, string contract, string address, bool noDecimalPoint = false)
  {
    BigInteger balanceOf = await ERC20.BalanceOf(chain, network, contract, address);

    if (noDecimalPoint)
    {
      return ((double)balanceOf);
    }

    BigInteger decimals = await ERC20.Decimals(chain, network, contract);
    return ((double)balanceOf) / System.Math.Pow(10.0, ((double)decimals));
  }

  public async Task<double> GetBalanceOf(string symbol, string address, bool noDecimalPoint = false)
  {
    if (!m_cryptoProject)
    {
      Debug.LogWarning("GPGCryptoManager Error: m_cryptoProject was null");
      return -1.0;
    }

    if (!m_cryptoProject.m_tokensMap.ContainsKey(symbol))
    {
      Debug.LogWarning("GPGCryptoManager Error: " + symbol + "token was not found, please adde it to the cryptoProject scriptable object");
      return 0.0;
    }

    if (!m_cryptoProject.m_tokensMap[symbol].implemented)
    {
      return 0.0;
    }

    BigInteger balanceOf = await ERC20.BalanceOf(m_cryptoProject.m_chain,
                                                 m_cryptoProject.m_network,
                                                 m_cryptoProject.m_tokensMap[symbol].contract,
                                                 address);

    if (noDecimalPoint)
    {
      return ((double)balanceOf);
    }

    BigInteger decimals = await ERC20.Decimals(m_cryptoProject.m_chain,
                                               m_cryptoProject.m_network,
                                               m_cryptoProject.m_tokensMap[symbol].contract);
    return ((double)balanceOf) / System.Math.Pow(10.0, ((double)decimals));
  }

  private readonly string abi = "[ { \"inputs\": [ { \"internalType\": \"string\", \"name\": \"name_\", \"type\": \"string\" }, { \"internalType\": \"string\", \"name\": \"symbol_\", \"type\": \"string\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"constructor\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"owner\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"value\", \"type\": \"uint256\" } ], \"name\": \"Approval\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"from\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"to\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"value\", \"type\": \"uint256\" } ], \"name\": \"Transfer\", \"type\": \"event\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"owner\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" } ], \"name\": \"allowance\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"approve\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"account\", \"type\": \"address\" } ], \"name\": \"balanceOf\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"decimals\", \"outputs\": [ { \"internalType\": \"uint8\", \"name\": \"\", \"type\": \"uint8\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"subtractedValue\", \"type\": \"uint256\" } ], \"name\": \"decreaseAllowance\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"addedValue\", \"type\": \"uint256\" } ], \"name\": \"increaseAllowance\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"name\", \"outputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"symbol\", \"outputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"totalSupply\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"recipient\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"transfer\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"sender\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"recipient\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"transferFrom\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" } ]";
  public async Task<string> Transfer(string toAccount, string contract, double amount)
  {
#if UNITY_WEBGL
    BigInteger decimals = await ERC20.Decimals(m_cryptoProject.m_chain,
                                              m_cryptoProject.m_network,
                                              contract);
    amount = amount * System.Math.Pow(10.0, ((double)decimals));

    BigInteger bigAmount = new BigInteger(amount);

    // smart contract method to call
    string method = "transfer";
    // array of arguments for contract
    string[] obj = { toAccount, bigAmount.ToString() };
    string args = JsonConvert.SerializeObject(obj);
    // value in wei
    string value = "0";
    // gas limit OPTIONAL
    //string gasLimit = "75000";
    string gasLimit = "75000";
    // optional rpc url
    string rpc = "";
    // gas price OPTIONAL
    string gasPrice = await EVM.GasPrice(m_cryptoProject.m_chain, m_cryptoProject.m_network, rpc);

    GPGTransaction newTransaction = new GPGTransaction();

    m_transactionsToConfirmByUser++;
    // connects to user's browser wallet (metamask) to send a transaction
    try
    {
      string response = await Web3GL.SendContract(method, abi, contract, args, value, gasLimit, gasPrice);
      newTransaction.transactionID = response;
      m_pendingTransactions.Add(response, newTransaction);
#if CD_LOGS
      Debug.Log(response);
#endif
      m_transactionsToConfirmByUser--;
    }
    catch (Exception e)
    {
      m_transactionsToConfirmByUser--;
      Debug.Log("FAILED");
      Debug.LogException(e, this);
      return "Error";
    }

    return newTransaction.transactionID;
#else
    return "Error";
#endif
  }

  public async Task<string> GetTransactionStatus(string transaction)
  {
    string txConfirmed = await EVM.TxStatus(m_cryptoProject.m_chain, m_cryptoProject.m_network, transaction);
#if CD_LOGS
    Debug.Log("status: " + txConfirmed); // success, fail, pending
#endif
    return txConfirmed;
  }

  public static string ConvertToShortAddress(string address)
  {
    int addressLenght = address.Length;

    string shortAddress = "(null)";
    if (addressLenght >= 5)
    {
      shortAddress = "(" +
                     address[0] +
                     address[1] +
                     address[2] +
                     address[3] +
                     address[4] +
                     "..." +
                     address[address.Length - 4] +
                     address[address.Length - 3] +
                     address[address.Length - 2] +
                     address[address.Length - 1] +
                     ")";
    }
    else
    {
      shortAddress = "Invalid Address";
    }

    return shortAddress;
  }

  public async Task<string> SignMessage(string message)
  {

    string response = "";
    try
    {
#if UNITY_WEBGL
      response = await Web3GL.Sign(message);
#else
      response = await Web3Wallet.Sign(message);
#endif
#if CD_LOGS
      Debug.Log("SignMessage response: " + response);
#endif
    }
    catch (Exception e)
    {
      Debug.LogException(e, this);
      response = "Error";
    }

    return response;
  }

  public async Task<bool> ERC1155HasNFT(string chain, string network, string contract, string tokenId, string account)
  {
    BigInteger balanceOf = await ERC1155.BalanceOf(chain, network, contract, account, tokenId);
    print("ERC1155HasNFT: " + balanceOf);
    return !balanceOf.IsZero;
  }

  public async Task<bool> ERC1155HasNFT(GPG_NFT_DESC nftDesc, string account)
  {
    BigInteger balanceOf = await ERC1155.BalanceOf(nftDesc.chain, nftDesc.network, nftDesc.contract, account, nftDesc.tokenID);
    print("ERC1155HasNFT: " + balanceOf);
    return !balanceOf.IsZero;
  }

  public async Task<int> ERC1155GetNFTCount(string chain, string network, string contract, string tokenId, string account)
  {
    BigInteger balanceOf = await ERC1155.BalanceOf(chain, network, contract, account, tokenId);
    print("ERC1155HasNFT: " + balanceOf);
    return ((int)balanceOf);
  }

  public async Task<int> ERC1155GetNFTCount(GPG_NFT_DESC nftDesc, string account)
  {
    BigInteger balanceOf = await ERC1155.BalanceOf(nftDesc.chain, nftDesc.network, nftDesc.contract, account, nftDesc.tokenID);
    print("ERC1155HasNFT: " + balanceOf);
    return ((int)balanceOf);
  }

}
