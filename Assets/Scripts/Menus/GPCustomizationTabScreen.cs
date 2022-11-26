using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPCustomizationTabScreen : GPGUIScreen
{
    public enum GP_CSTOMIZATION_TYPE
    {
        kSkin,
        kEyes,
        kMouth,
        kHair,
        kHorns,
        kWear,
        kGloves,
        kTail
    }

    public GP_CSTOMIZATION_TYPE m_type;

    [Header("Screen references")]
    public GPDummyCustomizationScreen m_customization;

    [Header("Customization menu references")]
    public Transform m_container; // where parts buttons will be parented
    public List<GPDummyPartDesc> m_parts;
    public GPDummyPartBlock m_blockPrefab; // part button prefab
    [HideInInspector]
    public List<GPDummyPartBlock> m_partBlocks = new List<GPDummyPartBlock>(); // store the instanced part buttons
    [HideInInspector]
    public GPDummyPartBlock m_selectedBlock = null;

    [Header("Equip settings")]
    public bool m_allowUnequip = true;
    public bool m_equipDefaultOnStart = false;
    public int m_defaultPartIdx = 0;

    [Header("Sound settings")]
    public AudioClip m_equipSFX;
    public AudioClip m_unEquipSFX;

    [Header("Dummy transform settings")]
    public Vector3 m_dummyRotation = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_parts.Count; i++)
        {
            GPDummyPartBlock newBlock = Instantiate(m_blockPrefab, m_container);
            newBlock.DisplayPart(m_parts[i]);
            newBlock.OnSelectedEvent.AddListener(OnBlockSelected);
            m_partBlocks.Add(newBlock);
        }

    }

    public override void Show()
    {
        base.Show();

        //Equip default parts
        if (m_equipDefaultOnStart && m_selectedBlock == null)
        {
            m_selectedBlock = m_partBlocks[m_defaultPartIdx];
            Debug.Log(m_selectedBlock.gameObject);
            m_selectedBlock.TogglePin(true);
            m_customization.m_customizationSlot.EquipCustomPart(m_partBlocks[m_defaultPartIdx].m_partDesc, false);
        }

        m_customization.m_customizationSlot.Rotate(m_dummyRotation);
    }

    /// <summary>
    /// Called when a dummy part button is clicked.
    /// Equips the dummy part to the dummy
    /// </summary>
    /// <param name="block"></param>
    public void OnBlockSelected(GPDummyPartBlock block)
    {
        if (m_selectedBlock)
        {
            m_selectedBlock.TogglePin(false);
            m_customization.m_customizationSlot.UnequipCustomPart(m_selectedBlock.m_partDesc);
        }

        // if its the same part then do not activate it again (click equip, click again to unequip)
        if (m_selectedBlock != block || !m_allowUnequip)
        {
            m_selectedBlock = block;
            m_selectedBlock.TogglePin(true);

            m_customization.m_customizationSlot.EquipCustomPart(block.m_partDesc);

            TanksMP.AudioManager.Play2D(m_equipSFX);
        }
        else
        {
            m_selectedBlock = null;
            TanksMP.AudioManager.Play2D(m_unEquipSFX);
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
