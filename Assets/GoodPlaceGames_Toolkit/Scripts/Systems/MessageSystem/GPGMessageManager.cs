using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GPG_MESSAGE_TYPE
{
  kDefault,
  kWarning,
  kAutentication,
  kNarrow,
}

public class GPGMessageManager : MonoBehaviour
{
  public GameObject m_canvas;
  public GameObject m_messagePrefab;
  public GameObject m_warningMessagePrefab;
  public GameObject m_autenticationMessagePrefab;
  public GameObject m_confirmationWindow;
  public GameObject m_narrowMessagePrefab;

  public static GPGMessageManager m_instance;

  private void Awake()
  {
    if (m_instance == null)
    {
      m_instance = this;
      DontDestroyOnLoad(m_instance);
    }
    else
    {
      Destroy(gameObject);
      return;
    }
  }

  // Start is called before the first frame update
  void Start()
  {

  }

  public GPGMessageWindow ShowMessage(string message, GPG_MESSAGE_TYPE type = GPG_MESSAGE_TYPE.kDefault)
  {
    GameObject prefabToUse = m_messagePrefab;
    if (type == GPG_MESSAGE_TYPE.kWarning)
    {
      prefabToUse = m_warningMessagePrefab;
    }
    else if (type == GPG_MESSAGE_TYPE.kAutentication)
    {
      prefabToUse = m_autenticationMessagePrefab;
    }
    else if (type == GPG_MESSAGE_TYPE.kNarrow)
    {
      prefabToUse = m_narrowMessagePrefab;
    }
    GPGMessageWindow messageWindow = Instantiate(prefabToUse, m_canvas.transform).GetComponent<GPGMessageWindow>();
    messageWindow.Show();
    messageWindow.SetMessage(message);
    return messageWindow;
  }

  public GPGConfirmationWindow ShowConfirmationMessage(string message, string headline)
  {
    GameObject prefabToUse = m_confirmationWindow;
    GPGConfirmationWindow messageWindow = Instantiate(prefabToUse, m_canvas.transform).GetComponent<GPGConfirmationWindow>();
    messageWindow.Show();
    messageWindow.SetMessage(message);
    messageWindow.SetHeadline(headline);
    return messageWindow;
  }

}
