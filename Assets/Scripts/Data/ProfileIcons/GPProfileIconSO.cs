using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GPProfileIconSO", menuName = "ScriptableObjects/GPProfileIconSO")]
public class GPProfileIconSO : ScriptableObject
{
    [SerializeField]
    [ScriptableObjectId]
    private string id;
    public string m_name;
    public Sprite m_sprite;
}
