using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UI<T> : MonoBehaviour where T : MonoBehaviour
{
    [Serializable]
    public struct TransitionData
    {
        [SerializeField]
        private Image imageBackground;

        [SerializeField]
        private GameObject contentMain;

        [SerializeField]
        private Color colorBackground;

        [SerializeField]
        private float duration;

        public Image ImageBackground { get => imageBackground; }

        public GameObject ContentMain { get => contentMain; }

        public Color ColorBackground { get => colorBackground; }

        public float Duration { get => duration; }
    }

    [Header("Transition Settings")]

    [SerializeField]
    private TransitionData onOpen;

    [SerializeField]
    private TransitionData onClose;

    public void Open()
    {
        OnBeforeOpen();

        gameObject.SetActive(true);

        OnAfterOpen();
    }

    public void Open(Action<T> OnBeforeRefresh)
    {
        OnBeforeOpen();

        RefreshUI(OnBeforeRefresh);

        gameObject.SetActive(true);

        OnAfterOpen();
    }

    public void Close()
    {
        OnBeforeClose(() =>
        {
            gameObject.SetActive(false);
        });

        OnAfterClose();
    }

    public void RefreshUI(Action<T> OnBeforeRefresh)
    {
        OnBeforeRefresh.Invoke(this as T);

        OnRefreshUI();
    }

    public void RefreshUI()
    {
        OnRefreshUI();
    }

    protected abstract void OnRefreshUI();

    private void OnBeforeOpen()
    {
        if (onOpen.ImageBackground)
        {
            onOpen.ImageBackground.color = new Color(0, 0, 0, 0);
        }

        if (onOpen.ContentMain)
        {
            onOpen.ContentMain.transform.localScale = Vector3.zero;
        }
    }

    private void OnAfterOpen()
    {
        if (onOpen.ImageBackground)
        {
            onOpen.ImageBackground.DOColor(onOpen.ColorBackground, onOpen.Duration);
        }

        if (onOpen.ContentMain)
        {
            onOpen.ContentMain.transform.DOScale(Vector3.one, onOpen.Duration);
        }
    }

    private void OnBeforeClose(Action onComplete)
    {
        if (onClose.ImageBackground)
        {
            onOpen.ImageBackground.DOColor(new Color(0, 0, 0, 0), onClose.Duration);
        }

        if (onClose.ContentMain)
        {
            onOpen.ContentMain.transform.DOScale(Vector3.zero, onClose.Duration).OnComplete(() => onComplete.Invoke());
        }
    }

    private void OnAfterClose() // TODO: not needed actually, remove in the future if there's no use for this
    {
        if (onClose.ImageBackground)
        {

        }

        if (onClose.ContentMain)
        {

        }
    }
}
