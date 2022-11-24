using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class GPDummySlotCard : MonoBehaviour
{
  public Transform m_dummyModelRef;
  public UnityEvent<GPDummySlotCard> OnSelectedEvent;

  // Start is called before the first frame update
  void Start()
  {

  }

  public void OnSelected()
  {
    if (OnSelectedEvent != null)
    {
      OnSelectedEvent.Invoke(this);
    }
  }

  public void EquipCustomPart(GPDummyPartDesc desc)
  {
    Transform part = RecursiveFindChild(m_dummyModelRef, desc.m_gameObjectName);
    part.gameObject.SetActive(true);
    if (desc.m_material != null)
    {
      part.GetComponent<Renderer>().material = desc.m_material;
    }
  }

  public void ReplaceModelObject(Transform newModelObject)
  {
    Transform newInstance = Instantiate(newModelObject, m_dummyModelRef.parent);
    newInstance.localPosition = m_dummyModelRef.localPosition;
    newInstance.localRotation = m_dummyModelRef.localRotation;
    newInstance.localScale = m_dummyModelRef.localScale;
    Destroy(m_dummyModelRef.gameObject);
    m_dummyModelRef = newInstance;
  }

  Transform RecursiveFindChild(Transform parent, string childName)
  {
    foreach (Transform child in parent)
    {
      if (child.name == childName)
      {
        return child;
      }
      else
      {
        Transform found = RecursiveFindChild(child, childName);
        if (found != null)
        {
          return found;
        }
      }
    }
    return null;
  }
}
