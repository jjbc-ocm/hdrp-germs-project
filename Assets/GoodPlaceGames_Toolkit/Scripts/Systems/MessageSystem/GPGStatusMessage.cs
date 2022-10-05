using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GPGStatusMessage : GPGMessageWindow
{
  public TextMeshProUGUI m_headline;
  public Image m_pendingImage;
  public Image m_completedImage;
  public Image m_cancelledImage;

  // Start is called before the first frame update
  void Start()
  {
    m_pendingImage.gameObject.SetActive(true);
    m_completedImage.gameObject.SetActive(false);
    m_cancelledImage.gameObject.SetActive(false);
  }

  // Update is called once per frame
  void Update()
  {

  }

  public void SetPendingState()
  {
    m_pendingImage.gameObject.SetActive(true);
    m_completedImage.gameObject.SetActive(false);
    m_cancelledImage.gameObject.SetActive(false);
  }

  public void SetCompletedState()
  {
    m_pendingImage.gameObject.SetActive(false);
    m_completedImage.gameObject.SetActive(true);
    m_cancelledImage.gameObject.SetActive(false);
  }

  public void SetCancelledState()
  {
    m_pendingImage.gameObject.SetActive(false);
    m_completedImage.gameObject.SetActive(false);
    m_cancelledImage.gameObject.SetActive(true);
  }

  public void SetHeadline(string headline)
  {
    m_headline.text = headline;
  }
}
