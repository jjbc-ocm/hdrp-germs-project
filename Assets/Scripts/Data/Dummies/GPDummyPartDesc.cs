using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GPDummyPartDesc", menuName = "ScriptableObjects/GPDummyPartDesc")]
public class GPDummyPartDesc : ScriptableObject
{
  public string m_gameObjectName;
  public Sprite m_displayIcon;
}
