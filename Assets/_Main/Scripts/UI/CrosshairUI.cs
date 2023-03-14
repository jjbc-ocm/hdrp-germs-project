using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairUI : Singleton<CrosshairUI>
{
    private Vector3 target;

    public Vector3 Target { set => target = value; }

    private void Update()
    {
        var screenPoint = GameCameraManager.Instance.MainCamera.WorldToScreenPoint(target);

        (transform as RectTransform).position = screenPoint;
    }
}
