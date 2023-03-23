using DamageNumbersPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : Singleton<PopupManager>
{
    [SerializeField]
    private RectTransform canvas;

    [SerializeField]
    private DamageNumber popupDamage;

    [SerializeField]
    private DamageNumber popupGold;

    [SerializeField]
    private DamageNumber popupHeal;

    public void ShowDamage(int amount, Vector3 worldPos)
    {
        var newDamage = popupDamage.Spawn(Vector3.zero, amount);

        Vector3 pos = GameCameraManager.Instance.MainCamera.WorldToScreenPoint(worldPos);

        newDamage.SetAnchoredPosition(canvas, pos);
    }

    public void ShowGold(int amount, Vector3 worldPos)
    {
        var newDamage = popupGold.Spawn(Vector3.zero, amount);

        Vector3 pos = GameCameraManager.Instance.MainCamera.WorldToScreenPoint(worldPos);

        newDamage.SetAnchoredPosition(canvas, pos);
    }

    public void ShowHeal(int amount, Vector3 worldPos)
    {
        var newDamage = popupHeal.Spawn(Vector3.zero, amount);

        Vector3 pos = GameCameraManager.Instance.MainCamera.WorldToScreenPoint(worldPos);

        newDamage.SetAnchoredPosition(canvas, pos);
    }
}
