using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonUI : UI<ButtonUI>, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Sound Settings")]

    [SerializeField]
    private AudioClip soundClick;

    [Header("Tween Settings")]

    [SerializeField]
    private float tweenDuration;

    [SerializeField]
    private Ease tweenEase;

    private float baseScale;

    #region Unity

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (tweenEase != Ease.Unset)
            {
                transform.DOScale(baseScale, tweenDuration).SetEase(tweenEase);
            }

            AudioManager.Instance.Play2D(soundClick);
        });

        baseScale = transform.localScale.x;
    }

    #endregion

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tweenEase != Ease.Unset)
        {
            transform.DOScale(baseScale * 1.2f, tweenDuration).SetEase(tweenEase);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tweenEase != Ease.Unset)
        {
            transform.DOScale(baseScale, tweenDuration).SetEase(tweenEase);
        }
    }

    protected override void OnRefreshUI()
    {

    }

    
}
