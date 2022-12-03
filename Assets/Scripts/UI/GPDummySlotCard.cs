using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class GPDummySlotCard : MonoBehaviour
{
    public Transform m_dummyModelRef;
    public UnityEvent<GPDummySlotCard> OnClickedEvent;
    public UnityEvent<GPDummySlotCard> OnToggledEvent;
    public Button m_toggle;
    public Image m_selectedSprite;
    public bool m_selected = false;
    Vector3 m_originalScale;
    Vector3 m_originalLocalEuler;
    public GPDummyData m_savedData;

    void Awake()
    {
        m_originalScale = m_dummyModelRef.localScale;
        m_originalLocalEuler = m_dummyModelRef.localEulerAngles;

        ToggleSelected(m_selected);
    }

    /// <summary>
    /// Called whe the dummy image is clicked.
    /// </summary>
    public void OnClicked()
    {
        if (OnClickedEvent != null)
        {
            OnClickedEvent.Invoke(this);
        }
    }

    /// <summary>
    /// Called whe the toggle is clicked.
    /// </summary>
    public void OnToggled()
    {
        m_selected = !m_selected;
        m_selectedSprite.enabled = m_selected;

        if (OnToggledEvent != null)
        {
            OnToggledEvent.Invoke(this);
        }
    }

    public void ToggleSelected(bool selected)
    {
        m_selected = selected;
        m_selectedSprite.enabled = m_selected;
    }

    /// <summary>
    /// Activates a dummy part on the dummy model.
    /// </summary>
    /// <param name="desc"></param>
    /// <param name="animate"></param>
    public void EquipCustomPart(GPDummyPartDesc desc, bool animate = true)
    {
        if (desc == null)
        {
            return;
        }
        Transform part = RecursiveFindChild(m_dummyModelRef, desc.m_gameObjectName);
        part.gameObject.SetActive(true);
        if (desc.m_material != null)
        {
            part.GetComponent<Renderer>().material = desc.m_material;
        }

        if (animate)
        {
            LeanTween.scale(m_dummyModelRef.gameObject, m_originalScale - (Vector3.one * 0.2f), 0.4f).setEasePunch();
        }
    }

    /// <summary>
    /// Deactivates a dummy part on the dummy model
    /// </summary>
    /// <param name="desc"></param>
    public void UnequipCustomPart(GPDummyPartDesc desc)
    {
        Transform part = RecursiveFindChild(m_dummyModelRef, desc.m_gameObjectName);
        part.gameObject.SetActive(false);

        m_dummyModelRef.gameObject.transform.localScale = m_originalScale;
        LeanTween.scale(m_dummyModelRef.gameObject, m_originalScale - (Vector3.one * 0.2f), 0.4f).setEasePunch();
    }

    /// <summary>
    /// Replaces the dummy model with a completly new one.
    /// Usefoll to replace the model with the one crated on the chustomization menu o the set a default model.
    /// </summary>
    /// <param name="newModelObject"></param>
    public void ReplaceModelObject(Transform newModelObject)
    {
        Transform newInstance = Instantiate(newModelObject, m_dummyModelRef.parent);
        newInstance.localPosition = m_dummyModelRef.localPosition;
        newInstance.localRotation = m_dummyModelRef.localRotation;
        newInstance.localScale = m_dummyModelRef.localScale;
        Destroy(m_dummyModelRef.gameObject);
        m_dummyModelRef = newInstance;
    }

    public void ChangeAppearance(GPDummyData data)
    {
        if (data == null)
        {
            return;
        }
        EquipCustomPart(data.m_skin);
        EquipCustomPart(data.m_eye);
        EquipCustomPart(data.m_mouth);
        EquipCustomPart(data.m_hair);
        EquipCustomPart(data.m_horns);
        EquipCustomPart(data.m_wear);
        EquipCustomPart(data.m_gloves);
        EquipCustomPart(data.m_tail);
    }

    public void Rotate(Vector3 newRotation)
    {
        //m_dummyModelRef.localEulerAngles = m_originalLocalEuler;
        LeanTween.rotateLocal(m_dummyModelRef.gameObject, newRotation, 0.4f).setEaseSpring();
    }

    /// <summary>
    /// Find a nested child of a transform that matchs the given child name.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="childName"></param>
    /// <returns></returns>
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
