using UnityEngine;

public class WaterWavee : MonoBehaviour
{
    public GameObject waterPlane;
    public float waveHeight = 0.1f;
    public float waveSpeed = 0.05f;
    private Mesh waterPlaneMesh;

    void Start()
    {
        waterPlaneMesh = waterPlane.GetComponent<MeshFilter>().mesh;
    }

    void UpdateWaves()
    {
        Vector3[] vertices = waterPlaneMesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y = Mathf.PerlinNoise(vertices[i].x * waveSpeed + Time.time, vertices[i].z * waveSpeed + Time.time) * waveHeight;
        }

        waterPlaneMesh.vertices = vertices;
    }

    void Update()
    {
        UpdateWaves();
    }
}