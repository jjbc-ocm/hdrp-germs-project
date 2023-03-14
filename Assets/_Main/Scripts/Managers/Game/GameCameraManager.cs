using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GameCameraManager : Singleton<GameCameraManager>
{
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;

    private Camera mainCamera;

    public Camera MainCamera { get => mainCamera; }

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        var targetFOV = Player.Mine.Input.IsSprint ? 90f : 75f;

        virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, targetFOV, Time.deltaTime * 2f);
    }

    public void SetTarget(Transform target)
    {
        virtualCamera.Follow = target;

        virtualCamera.LookAt = target;
    }
}
