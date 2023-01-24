using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPNumberSpawnerSystem : MonoBehaviour
{
    public GPFloatingNumberUI m_goldNumberPrefab;
    public Canvas m_canvas;
    public float m_numberLifetime = 1.0f;

    public static GPNumberSpawnerSystem m_instance;

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SpawnGoldNumber(int goldAmount, Vector3 goldOriginPosition)
    {
        GPFloatingNumberUI instancedNumber = Instantiate(m_goldNumberPrefab, m_canvas.transform);
        instancedNumber.SetNumber(goldAmount);
        instancedNumber.SetPosition(goldOriginPosition);
        Destroy(instancedNumber.gameObject, m_numberLifetime);
    }

}
