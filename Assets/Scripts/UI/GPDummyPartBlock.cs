using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GPDummyPartBlock : MonoBehaviour
{
  public Image m_iconSprite;
  public Image m_selectedPinImage;
  public Button m_button;
  [HideInInspector]
  public GPDummyPartDesc m_partDesc;

  public UnityEvent<GPDummyPartBlock> OnSelectedEvent;

  // Start is called before the first frame update
  void Start()
  {
    m_button.onClick.AddListener(OnSelected);
    TogglePin(false);
  }

  public void DisplayPart(GPDummyPartDesc desc)
  {
    m_partDesc = desc;
    m_iconSprite.sprite = desc.m_displayIcon;
  }

  public void TogglePin(bool active)
  {
    m_selectedPinImage.gameObject.SetActive(active);
  }

  public void OnSelected()
  {
    if (OnSelectedEvent != null)
    {
      OnSelectedEvent.Invoke(this);
    }
  }
}
