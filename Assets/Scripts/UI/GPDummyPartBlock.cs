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
    public DummyPartSO m_partDesc;

    public UnityEvent<GPDummyPartBlock> OnSelectedEvent;

    // Start is called before the first frame update
    void Start()
    {
        m_button.onClick.AddListener(OnSelected);
        //TogglePin(false);
    }

    /// <summary>
    /// Changes the icon of the body part button using the given dummy part description
    /// </summary>
    /// <param name="desc"></param>
    public void DisplayPart(DummyPartSO desc)
    {
        m_partDesc = desc;
        m_iconSprite.sprite = desc.m_displayIcon;
    }

    /// <summary>
    /// To activate/deactivate the pin that let the user know if that part is currently equipped.
    /// </summary>
    /// <param name="active"></param>
    public void TogglePin(bool active)
    {
        m_selectedPinImage.gameObject.SetActive(active);
    }


    /// <summary>
    /// Called when the body part button is clicked.
    /// </summary>
    public void OnSelected()
    {
        if (OnSelectedEvent != null)
        {
            OnSelectedEvent.Invoke(this);
        }
    }
}
