using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPCustomizationTabScreen : GPGUIScreen
{
    public GP_DUMMY_PART_TYPE m_type;

    [Header("Screen references")]
    public GPDummyCustomizationScreen m_customization;

    [Header("Customization menu references")]
    public Transform m_container; // where parts buttons will be parented
    private List<GPDummyPartDesc> m_parts = new List<GPDummyPartDesc>();
    public GPDummyPartBlock m_blockPrefab; // part button prefab
    [HideInInspector]
    public List<GPDummyPartBlock> m_partBlocks = new List<GPDummyPartBlock>(); // store the instanced part buttons
    [HideInInspector]
    public GPDummyPartBlock m_selectedBlock = null;

    [Header("Equip settings")]
    public bool m_allowUnequip = true;
    public bool m_equipDefaultOnStart = false;
    //public int m_defaultPartIdx = 0;

    [Header("Sound settings")]
    public AudioClip m_equipSFX;
    public AudioClip m_unEquipSFX;

    [Header("Dummy transform settings")]
    public Vector3 m_dummyRotation = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        GPPlayerProfile.m_instance.OnDummyPartsModifiedEvent.AddListener(UpdateAvailableParts);
        UpdateAvailableParts();
    }

    public override void Show()
    {
        base.Show();

        //Equip default parts
        /*if (m_equipDefaultOnStart && m_selectedBlock == null)
        {
            m_selectedBlock = m_partBlocks[m_defaultPartIdx];
            Debug.LogError("m_defaultPartIdx: " + m_defaultPartIdx);
            m_selectedBlock.TogglePin(true);
            m_customization.m_customizationSlot.EquipCustomPart(m_partBlocks[m_defaultPartIdx].m_partDesc, false);
        }*/

        m_customization.m_customizationSlot.Rotate(m_dummyRotation);

        UpdatePins();
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

            switch (m_selectedBlock.m_partDesc.m_type)
            {
                case GP_DUMMY_PART_TYPE.kSkin:
                    m_customization.m_dummyScreen.m_selectedSlot.m_savedData.m_skin = null;
                    break;
                case GP_DUMMY_PART_TYPE.kEye:
                    m_customization.m_dummyScreen.m_selectedSlot.m_savedData.m_eye = null;
                    break;
                case GP_DUMMY_PART_TYPE.kMouth:
                    m_customization.m_dummyScreen.m_selectedSlot.m_savedData.m_mouth = null;
                    break;
                case GP_DUMMY_PART_TYPE.kHair:
                    m_customization.m_dummyScreen.m_selectedSlot.m_savedData.m_hair = null;
                    break;
                case GP_DUMMY_PART_TYPE.kHorn:
                    m_customization.m_dummyScreen.m_selectedSlot.m_savedData.m_horns = null;
                    break;
                case GP_DUMMY_PART_TYPE.kWear:
                    m_customization.m_dummyScreen.m_selectedSlot.m_savedData.m_wear = null;
                    break;
                case GP_DUMMY_PART_TYPE.kGlove:
                    m_customization.m_dummyScreen.m_selectedSlot.m_savedData.m_gloves = null;
                    break;
                case GP_DUMMY_PART_TYPE.kTail:
                    m_customization.m_dummyScreen.m_selectedSlot.m_savedData.m_tail = null;
                    break;
                default:
                    break;
            }
        }

        // if its the same part then do not activate it again (click equip, click again to unequip)
        if (m_selectedBlock != block || !m_allowUnequip)
        {
            m_selectedBlock = block;
            m_selectedBlock.TogglePin(true);

            m_customization.m_customizationSlot.EquipCustomPart(block.m_partDesc);

            switch (block.m_partDesc.m_type)
            {
                case GP_DUMMY_PART_TYPE.kSkin:
                    m_customization.m_dummyScreen.m_selectedSlot.m_savedData.m_skin = block.m_partDesc;
                    break;
                case GP_DUMMY_PART_TYPE.kEye:
                    m_customization.m_dummyScreen.m_selectedSlot.m_savedData.m_eye = block.m_partDesc;
                    break;
                case GP_DUMMY_PART_TYPE.kMouth:
                    m_customization.m_dummyScreen.m_selectedSlot.m_savedData.m_mouth = block.m_partDesc;
                    break;
                case GP_DUMMY_PART_TYPE.kHair:
                    m_customization.m_dummyScreen.m_selectedSlot.m_savedData.m_hair = block.m_partDesc;
                    break;
                case GP_DUMMY_PART_TYPE.kHorn:
                    m_customization.m_dummyScreen.m_selectedSlot.m_savedData.m_horns = block.m_partDesc;
                    break;
                case GP_DUMMY_PART_TYPE.kWear:
                    m_customization.m_dummyScreen.m_selectedSlot.m_savedData.m_wear = block.m_partDesc;
                    break;
                case GP_DUMMY_PART_TYPE.kGlove:
                    m_customization.m_dummyScreen.m_selectedSlot.m_savedData.m_gloves = block.m_partDesc;
                    break;
                case GP_DUMMY_PART_TYPE.kTail:
                    m_customization.m_dummyScreen.m_selectedSlot.m_savedData.m_tail = block.m_partDesc;
                    break;
                default:
                    break;
            }

            AudioManager.Instance.Play2D(m_equipSFX);
        }
        else
        {
            m_selectedBlock = null;
            AudioManager.Instance.Play2D(m_unEquipSFX);
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

    public GPDummyPartBlock FindBlockOfPartDesc(GPDummyPartDesc desc)
    {
        for (int i = 0; i < m_partBlocks.Count; i++)
        {
            if (m_partBlocks[i].m_partDesc == desc)
            {
                return m_partBlocks[i];
            }
        }

        return null;
    }

    void UpdatePins()
    {
        //disable other pins
        foreach (var blockPart in m_partBlocks)
        {
            blockPart.TogglePin(false);
        }

        //activate the pin of the equipped part
        GPDummyPartBlock block = null;
        switch (m_type)
        {
            case GP_DUMMY_PART_TYPE.kSkin:
                {
                    block = FindBlockOfPartDesc(m_customization.m_dummyScreen.m_selectedSlot.m_savedData.m_skin);
                    break;
                }
            case GP_DUMMY_PART_TYPE.kEye:
                {

                    block = FindBlockOfPartDesc(m_customization.m_dummyScreen.m_selectedSlot.m_savedData.m_eye);
                    break;
                }
            case GP_DUMMY_PART_TYPE.kMouth:
                {

                    block = FindBlockOfPartDesc(m_customization.m_dummyScreen.m_selectedSlot.m_savedData.m_mouth);
                    break;
                }
            case GP_DUMMY_PART_TYPE.kHair:
                {

                    block = FindBlockOfPartDesc(m_customization.m_dummyScreen.m_selectedSlot.m_savedData.m_hair);
                    break;
                }
            case GP_DUMMY_PART_TYPE.kHorn:
                {

                    block = FindBlockOfPartDesc(m_customization.m_dummyScreen.m_selectedSlot.m_savedData.m_horns);
                    break;
                }
            case GP_DUMMY_PART_TYPE.kWear:
                {

                    block = FindBlockOfPartDesc(m_customization.m_dummyScreen.m_selectedSlot.m_savedData.m_wear);
                    break;
                }
            case GP_DUMMY_PART_TYPE.kGlove:
                {

                    block = FindBlockOfPartDesc(m_customization.m_dummyScreen.m_selectedSlot.m_savedData.m_gloves);
                    break;
                }
            case GP_DUMMY_PART_TYPE.kTail:
                {
                    block = FindBlockOfPartDesc(m_customization.m_dummyScreen.m_selectedSlot.m_savedData.m_tail);
                    break;
                }
            default:
                break;
        }

        if (block)
        {
            m_selectedBlock = block;
            block.TogglePin(true);
        }
    }

    void UpdateAvailableParts()
    {
        //m_parts.Clear();
        switch (m_type)
        {
            case GP_DUMMY_PART_TYPE.kSkin:
                m_parts = GPPlayerProfile.m_instance.m_dummySkins;
                break;
            case GP_DUMMY_PART_TYPE.kEye:
                m_parts = GPPlayerProfile.m_instance.m_dummyEyes;
                break;
            case GP_DUMMY_PART_TYPE.kMouth:
                m_parts = GPPlayerProfile.m_instance.m_dummyMouths;
                break;
            case GP_DUMMY_PART_TYPE.kHair:
                m_parts = GPPlayerProfile.m_instance.m_dummyHairs;
                break;
            case GP_DUMMY_PART_TYPE.kHorn:
                m_parts = GPPlayerProfile.m_instance.m_dummyHorns;
                break;
            case GP_DUMMY_PART_TYPE.kWear:
                m_parts = GPPlayerProfile.m_instance.m_dummyWears;
                break;
            case GP_DUMMY_PART_TYPE.kGlove:
                m_parts = GPPlayerProfile.m_instance.m_dummyGloves;
                break;
            case GP_DUMMY_PART_TYPE.kTail:
                m_parts = GPPlayerProfile.m_instance.m_dummyTails;
                break;
            default:
                break;
        }

        //clear old blocks
        foreach (var item in m_partBlocks)
        {
            Destroy(item.gameObject);
        }

        m_partBlocks.Clear();
        for (int i = 0; i < m_parts.Count; i++)
        {
            GPDummyPartBlock newBlock = Instantiate(m_blockPrefab, m_container);
            newBlock.DisplayPart(m_parts[i]);
            newBlock.OnSelectedEvent.AddListener(OnBlockSelected);
            m_partBlocks.Add(newBlock);
        }
    }
}
