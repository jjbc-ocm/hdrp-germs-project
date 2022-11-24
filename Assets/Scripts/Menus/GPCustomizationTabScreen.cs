using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPCustomizationTabScreen : GPGUIScreen
{
  public GPDummyCustomizationScreen m_customization;
  public Transform m_container;
  public List<GPDummyPartDesc> m_parts;
  public GPDummyPartBlock m_blockPrefab;
  [HideInInspector]
  public List<GPDummyPartBlock> m_partBlocks = new List<GPDummyPartBlock>();
  [HideInInspector]
  public GPDummyPartBlock m_selectedBlock = null;

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

    if (m_partBlocks.Count > 0)
    {
      m_selectedBlock = m_partBlocks[0];
    }
  }

  public void OnBlockSelected(GPDummyPartBlock block)
  {
    if (m_selectedBlock)
    {
      m_selectedBlock.TogglePin(false);
      RecursiveFindChild(m_customization.m_customizationSlot.m_dummyModelRef, m_selectedBlock.m_partDesc.m_gameObjectName).gameObject.SetActive(false);
    }
    m_selectedBlock = block;
    m_selectedBlock.TogglePin(true);

    Transform part = RecursiveFindChild(m_customization.m_customizationSlot.m_dummyModelRef, block.m_partDesc.m_gameObjectName);
    part.gameObject.SetActive(true);
    if (block.m_partDesc.m_material != null)
    {
      part.GetComponent<Renderer>().material = block.m_partDesc.m_material;
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
