using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapManager : MonoBehaviour
{
    [SerializeField]
    private GameObject minimapCamera;

    [SerializeField]
    private Vector3[] positions;

    [SerializeField]
    private Vector3[] rotations;

    void Start()
    {
        var index = PhotonNetwork.LocalPlayer.GetTeam();

        minimapCamera.transform.position = positions[index];

        minimapCamera.transform.eulerAngles = rotations[index];
    }
}
