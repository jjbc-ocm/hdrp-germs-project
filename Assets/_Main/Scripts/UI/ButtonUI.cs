using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonUI : UI<ButtonUI>
{
    [Header("Sound Settings")]

    [SerializeField]
    private AudioClip soundClick;

    protected override void OnRefreshUI()
    {

    }

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            AudioManager.Instance.Play2D(soundClick);
        });
    }
}
