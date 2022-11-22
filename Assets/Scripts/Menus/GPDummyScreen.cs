using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPDummyScreen : GPGUIScreen
{
  public GPGUIScreen m_dummySelectScreen;
  public GPGUIScreen m_dummyCustomizingScreen;

  // Start is called before the first frame update
  void Start()
  {

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

  public void OnDummySelected()
  {
    //Show customization screen for that dummy.
    m_dummySelectScreen.Hide();
    m_dummyCustomizingScreen.Show();
  }

  public void ReturnToDummySelection()
  {
    m_dummySelectScreen.Show();
    m_dummyCustomizingScreen.Hide();
  }
}
