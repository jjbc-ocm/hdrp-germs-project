using DamageNumbersPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : Singleton<PopupManager>
{
    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private DamageNumber popupDamage;

    [SerializeField]
    private DamageNumber popupGold;

    [SerializeField]
    private DamageNumber popupHeal;

    public void ShowDamage(int amount, Vector3 worldPos)
    {
        var newPopup = popupDamage.Spawn(Vector3.zero, amount);

        var pos = GameCameraManager.Instance.MainCamera.WorldToScreenPoint(worldPos);

        newPopup.SetAnchoredPosition(canvas.transform, pos);
    }

    public void ShowGold(int amount, Vector3 worldPos)
    {
        var newPopup = popupGold.Spawn(Vector3.zero, amount);

        var pos = GameCameraManager.Instance.MainCamera.WorldToScreenPoint(worldPos);

        newPopup.SetAnchoredPosition(canvas.transform, pos);
    }

    public void ShowHeal(int amount, Vector3 worldPos)
    {
        var newPopup = popupHeal.Spawn(Vector3.zero, amount);

        var pos = GameCameraManager.Instance.MainCamera.WorldToScreenPoint(worldPos);

        newPopup.SetAnchoredPosition(canvas.transform, pos);
    }
}
