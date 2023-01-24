using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GPFloatingNumberUI : MonoBehaviour
{
  public TextMeshProUGUI m_text;

  [Header("Position Settings")]
  public Vector3 m_offset;

  public void SetNumber(int number)
  {
    m_text.text = "+" + number.ToString();
  }

  public void SetPosition(Vector3 worldPos)
  {
    Vector3 pos = Camera.main.WorldToScreenPoint(worldPos + m_offset);

    if (transform.position != pos) { transform.position = pos; }
  }
}
