using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CrewUI : WindowUI<CrewUI>
{
    [SerializeField]
    private Transform cameraPos;

    public void OnHomeClick()
    {
        HomeUI.Instance.Open((self) => { });

        Close();
    }

    protected override void OnRefreshUI()
    {
        Camera.main.transform.DOMove(cameraPos.transform.position, 0.25f).SetEase(Ease.InOutBounce);
    }
}
