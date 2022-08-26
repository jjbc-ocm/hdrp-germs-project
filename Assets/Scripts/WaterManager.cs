using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManager : MonoBehaviour
{
    public static WaterManager Instance;

    [SerializeField]
    private MeshRenderer meshRenderer;

    private Vector3 posInit;

    private float deltaTime;


    private void Awake()
    {
        Instance = this;

        posInit = transform.position;

        print("TEST A: " + meshRenderer.material.GetFloat("Vector1_3849afd7c2eb484893a68ec694cc75b3"));
    }

    private void Update()
    {
        deltaTime += Time.deltaTime * 0.25f;

        transform.position = posInit + Vector3.up * Mathf.Sin(deltaTime);
    }
}
