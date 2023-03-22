using DG.Tweening;
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

    [Header("Tween Settings")]

    [SerializeField]
    private float tweenDurationClick;

    [SerializeField]
    private Ease tweenEaseClick;

    protected override void OnRefreshUI()
    {

    }

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (tweenEaseClick != Ease.Unset)
            {
                transform.DOScale(Vector3.one, tweenDurationClick).SetEase(tweenEaseClick);
            }

            AudioManager.Instance.Play2D(soundClick);
        });
    }
}
