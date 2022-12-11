using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapManager : MonoBehaviour
{
    [SerializeField]
    private Camera minimapCamera;

    [SerializeField]
    private Vector3[] positions;

    [SerializeField]
    private Vector3[] rotations;

    void Update()
    {
        var index = PhotonNetwork.LocalPlayer.GetTeam();

        if (index < 0) return;

        var offset = positions[index];

        minimapCamera.transform.eulerAngles = rotations[index];

        minimapCamera.transform.position = Vector3.zero;

        var targetPos = minimapCamera.ViewportToWorldPoint(new Vector3(0, 1, minimapCamera.nearClipPlane));

        minimapCamera.transform.localPosition = new Vector3(targetPos.x + offset.x, offset.y, targetPos.z + offset.z);

        
    }
}
