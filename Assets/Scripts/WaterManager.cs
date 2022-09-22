using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManager : MonoBehaviour
{
    public static WaterManager Instance;

    [SerializeField]
    private MeshRenderer meshRenderer;

    //private PhotonView photonView;

    private Vector3 posInit;

    private float deltaTime;


    private void Awake()
    {
        Instance = this;

        posInit = transform.position;
    }

    private void Update()
    {
        deltaTime += Time.deltaTime * 0.25f;

        //transform.position = posInit + Vector3.up * Mathf.Sin(deltaTime);
    }
}
