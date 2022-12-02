using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPDummyLoader : MonoBehaviour
{
    public Transform m_dummyModelRef;
    Vector3 m_originalScale;
    Vector3 m_originalLocalEuler;

    // Start is called before the first frame update
    void Start()
    {
        ChangeAppearance(GPPlayerProfile.m_instance.m_dummySlots[GPPlayerProfile.m_instance.m_currDummySlotIdx]);
    }

    public void ChangeAppearance(GPDummyData data)
    {
        EquipCustomPart(data.m_skin);
        EquipCustomPart(data.m_eye);
        EquipCustomPart(data.m_mouth);
        EquipCustomPart(data.m_hair);
        EquipCustomPart(data.m_horns);
        EquipCustomPart(data.m_wear);
        EquipCustomPart(data.m_gloves);
        EquipCustomPart(data.m_tail);
    }

    /// <summary>
    /// Activates a dummy part on the dummy model.
    /// </summary>
    /// <param name="desc"></param>
    /// <param name="animate"></param>
    public void EquipCustomPart(GPDummyPartDesc desc, bool animate = true)
    {
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
