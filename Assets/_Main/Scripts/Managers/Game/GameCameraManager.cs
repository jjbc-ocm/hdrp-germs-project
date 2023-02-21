using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GameCameraManager : MonoBehaviour
{
    public static GameCameraManager Instance;

    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;

    private Camera mainCamera;

    public Camera MainCamera { get => mainCamera;}

    private void Awake()
    {
        Instance = this;

        mainCamera = GetComponent<Camera>();
    }

    public void SetTarget(Transform target)
    {
        virtualCamera.Follow = target;

        virtualCamera.LookAt = target;
    }
}
