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

    [SerializeField]
    private float limitYaw;

    [SerializeField]
    private float limitPitch;

    private Camera mainCamera;

    private float targetYaw;

    private float targetPitch;

    public Camera MainCamera { get => mainCamera; }

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        var targetFOV = PlayerManager.Mine.Input.IsSprint ? 90f : 75f;

        virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, targetFOV, Time.deltaTime * 2f);
    }
    
    private void LateUpdate()
    {
        
        // if there is an input and camera position is not fixed
        if (InputManager.Instance.LookDelta.sqrMagnitude >= 0.01f)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = 1f;

            targetYaw += InputManager.Instance.LookDelta.x * deltaTimeMultiplier;
            targetPitch += InputManager.Instance.LookDelta.y * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        targetYaw = ClampAngle(targetYaw, -limitYaw, limitYaw);
        targetPitch = ClampAngle(targetPitch, -limitPitch, limitPitch);

        // Cinemachine will follow this target
        virtualCamera.Follow.transform.localRotation = Quaternion.Euler(targetPitch, targetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    public void SetTarget(Transform target)
    {
        virtualCamera.Follow = target;

        virtualCamera.LookAt = target;
    }
}
