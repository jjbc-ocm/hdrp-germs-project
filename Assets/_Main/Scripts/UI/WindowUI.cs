using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public abstract class WindowUI<T> : UI<T> where T : MonoBehaviour
{
    [SerializeField]
    private GameObject windowMain;

    [Header("Sound Settings")]

    [SerializeField]
    private AudioClip soundOpen;

    [SerializeField]
    private AudioClip soundClose;

    [Header("Tween Settings")]
    
    [SerializeField]
    private float tweenDuration;

    [SerializeField]
    private Ease tweenEaseOpen;

    [SerializeField]
    private Ease tweenEaseClose;

    public new void Open()
    {
        OnBeforeOpen();

        gameObject.SetActive(true);

        OnAfterOpen();
    }

    public new void Open(Action<T> OnBeforeRefresh)
    {
        OnBeforeOpen();

        RefreshUI(OnBeforeRefresh);

        gameObject.SetActive(true);

        OnAfterOpen();
    }

    public new void Close()
    {
        OnBeforeClose(() =>
        {
            gameObject.SetActive(false);
        });

        OnAfterClose();
    }




    private void OnBeforeOpen()
    {
        if (windowMain)
        {
            windowMain.transform.localScale = Vector3.zero;
        }

        if (soundOpen)
        {
            AudioManager.Play2D(soundOpen);
        }
    }

    private void OnAfterOpen()
    {
        if (windowMain)
        {
            windowMain.transform.DOScale(Vector3.one, tweenDuration).SetEase(tweenEaseOpen);
        }

        
    }

    private void OnBeforeClose(Action onComplete)
    {
        if (windowMain)
        {
            windowMain.transform.DOScale(Vector3.zero, tweenDuration).SetEase(tweenEaseClose).OnComplete(() => onComplete.Invoke());
        }

        if (soundOpen)
        {
            AudioManager.Play2D(soundClose);
        }
    }

    private void OnAfterClose()
    {
        
    }
}
