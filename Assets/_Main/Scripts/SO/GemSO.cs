using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dummy Pirates/Gem")]
public class GemSO : ScriptableObject
{
    [SerializeField]
    private int amount;

    [SerializeField]
    private Sprite sprite;

    public int Amount { get => amount; }

    public Sprite Sprite { get => sprite; }
}
