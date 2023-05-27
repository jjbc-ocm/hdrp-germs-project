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
        if (!m_view) return;

        var keys = m_view.Owner.GetDummyKeys();

        ChangeAppearance(DummyData.Create(keys));
    }

    public void ChangeAppearance(DummyData data)
    {
        foreach (var part in SOManager.Instance.DummyParts)
        {
            UnequipCustomPart(part.name);
        }

        EquipCustomPart(data.Skin);
        EquipCustomPart(data.Eye);
        EquipCustomPart(data.Mouth);
        EquipCustomPart(data.Head);
        EquipCustomPart(data.Wear);
        EquipCustomPart(data.Glove);
        EquipCustomPart(data.Tail);
    }


    public void EquipCustomPart(string name)
    {
        if (string.IsNullOrEmpty(name)) return;

        Transform part = RecursiveFindChild(m_dummyModelRef, name);

        if (part)
        {
            part.gameObject.SetActive(true);
        }
    }

    public void UnequipCustomPart(string name)
    {
        if (string.IsNullOrEmpty(name)) return;

        Transform part = RecursiveFindChild(m_dummyModelRef, name);

        if (part)
        {
            part.gameObject.SetActive(false);
        }
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
