using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UI<T> : MonoBehaviour where T : MonoBehaviour
{
    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Open(Action<T> OnBeforeRefresh)
    {
        RefreshUI(OnBeforeRefresh);

        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
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
}
