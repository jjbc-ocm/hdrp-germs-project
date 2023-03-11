using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GuideData : ScriptableObject
{
    [SerializeField]
    private string title;

    [SerializeField]
    [TextArea(5, 10)]
    private string text;

    [SerializeField]
    private Color colorFrame;

    [SerializeField]
    private Color colorBack;

    [SerializeField]
    private Color colorIcon;

    public string Title { get => title; }

    public string Text { get => text; }

    public Color ColorFrame { get => colorFrame;  }

    public Color ColorBack { get => colorBack; }

    public Color ColorIcon { get => colorIcon; }
}
