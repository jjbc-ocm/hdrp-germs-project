using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPDummyScreen : GPGUIScreen
{
  public GPGUIScreen m_dummySelectScreen;
  public GPDummyCustomizationScreen m_dummyCustomizingScreen;

  public List<GPDummySlotCard> m_dummySlots;
  GPDummySlotCard m_selectedSlot;

  // Start is called before the first frame update
  void Start()
  {
    m_dummyCustomizingScreen.m_customizationSlot.OnSelectedEvent.AddListener(ReturnToDummySelection);
    foreach (var slot in m_dummySlots)
    {
      slot.OnSelectedEvent.AddListener(OnDummySelected);
    }
  }

  public override void Show()
  {
    base.Show();
    m_dummySelectScreen.Show();
  }

  public override void Hide()
  {
    base.Hide();
  }

  public void OnDummySelected(GPDummySlotCard card)
  {
    m_selectedSlot = card;

    m_dummyCustomizingScreen.m_customizationSlot.ReplaceModelObject(m_selectedSlot.m_dummyModelRef);
    //Show customization screen for that dummy.
    m_dummySelectScreen.Hide();
    m_dummyCustomizingScreen.Show();
  }

  public void ReturnToDummySelection(GPDummySlotCard card)
  {
    m_dummySelectScreen.Show();
    m_dummyCustomizingScreen.Hide();

    if (m_selectedSlot)
    {
      m_selectedSlot.ReplaceModelObject(m_dummyCustomizingScreen.m_customizationSlot.m_dummyModelRef);
    }
  }
}
