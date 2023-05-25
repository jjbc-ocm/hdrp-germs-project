using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class GPDummyLoader : MonoBehaviour
{
    public Transform m_dummyModelRef;
    public PhotonView m_view;

    // Start is called before the first frame update
    void Start()
    {
        var keys = m_view.Owner.GetDummyKeys();

        var data = new GPDummyData(keys);

        ChangeAppearance(data);
    }

    public void ChangeAppearance(GPDummyData data)
    {
        EquipCustomPart(data.m_skin);
        EquipCustomPart(data.m_eye);
        EquipCustomPart(data.m_mouth);
        EquipCustomPart(data.m_head);
        EquipCustomPart(data.m_wear);
        EquipCustomPart(data.m_gloves);
        EquipCustomPart(data.m_tail);
    }

    /// <summary>
    /// Activates a dummy part on the dummy model.
    /// </summary>
    /// <param name="desc"></param>
    /// <param name="animate"></param>
    public void EquipCustomPart(DummyPartSO desc)
    {
        if (!desc)
        {
            return;
        }

        Transform part = RecursiveFindChild(m_dummyModelRef, desc.name);

        if (part)
        {
            part.gameObject.SetActive(true);

            /*if (desc.m_material != null)
            {
                part.GetComponent<Renderer>().material = desc.m_material;
            }*/
        }
    }

    /// <summary>
    /// Deactivates a dummy part on the dummy model
    /// </summary>
    /// <param name="desc"></param>
    public void UnequipCustomPart(DummyPartSO desc)
    {
        Transform part = RecursiveFindChild(m_dummyModelRef, desc.name);

        if (part != null)
        {
            part.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log(desc.name + " is not in the dummy parts");
        }
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
