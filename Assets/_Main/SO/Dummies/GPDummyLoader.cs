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
        List<string> dummyKeys = m_view.Owner.GetDummyKeys();
        GPDummyData loadedData = new GPDummyData();
        foreach (var key in dummyKeys)
        {
            if (key == null)
            {
                continue;
            }

            /*if (!GPItemsDB.m_instance.m_dummyPartsMap.ContainsKey(key))
            {
                continue;
            }*/

            var part = SOManager.Instance.DummyParts.FirstOrDefault(i => key == i.name);

            //DummyPartSO part = GPItemsDB.m_instance.m_dummyPartsMap[key];
            switch (part.m_type)
            {
                case GP_DUMMY_PART_TYPE.kSkin:
                    loadedData.m_skin = part;
                    break;
                case GP_DUMMY_PART_TYPE.kEye:
                    loadedData.m_eye = part;
                    break;
                case GP_DUMMY_PART_TYPE.kMouth:
                    loadedData.m_mouth = part;
                    break;
                case GP_DUMMY_PART_TYPE.kHair:
                    loadedData.m_hair = part;
                    break;
                case GP_DUMMY_PART_TYPE.kHorn:
                    loadedData.m_horns = part;
                    break;
                case GP_DUMMY_PART_TYPE.kWear:
                    loadedData.m_wear = part;
                    break;
                case GP_DUMMY_PART_TYPE.kGlove:
                    loadedData.m_gloves = part;
                    break;
                case GP_DUMMY_PART_TYPE.kTail:
                    loadedData.m_tail = part;
                    break;
                default:
                    break;
            }
        }

        ChangeAppearance(loadedData);
        //ChangeAppearance(GPPlayerProfile.m_instance.m_dummySlots[GPPlayerProfile.m_instance.m_currDummySlotIdx]);
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

            if (desc.m_material != null)
            {
                part.GetComponent<Renderer>().material = desc.m_material;
            }
        }
    }

    /// <summary>
    /// Deactivates a dummy part on the dummy model
    /// </summary>
    /// <param name="desc"></param>
    public void UnequipCustomPart(DummyPartSO desc)
    {
        Transform part = RecursiveFindChild(m_dummyModelRef, desc.name);
        part.gameObject.SetActive(false);
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
