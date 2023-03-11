using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPNumberSpawnerSystem : MonoBehaviour
{
    public GPFloatingNumberUI m_goldNumberPrefab;
    public GPFloatingNumberUI m_damageNumberPrefab;
    public GPFloatingNumberUI m_healNumberPrefab;
    public Canvas m_canvas;
    public Camera m_mainCamera;
    public float m_numberLifetime = 1.0f;
    public float m_randomOffsetMult = 2.0f;

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
        instancedNumber.SetNumber(goldAmount, "+");
        Vector3 randomOffset = new Vector3(Random.Range(-m_randomOffsetMult, m_randomOffsetMult),
                                           Random.Range(-m_randomOffsetMult, m_randomOffsetMult),
                                           Random.Range(-m_randomOffsetMult, m_randomOffsetMult));
        instancedNumber.SetPosition(goldOriginPosition + randomOffset, m_mainCamera);
    }

    public void SpawnDamageNumber(int amount, Vector3 originPosition)
    {
        // no need to spawn negative damage.
        // for some reason when shooting boolets player receives -1 damage.
        if (amount <= 0)
        {
            return;
        }

        GPFloatingNumberUI instancedNumber = Instantiate(m_damageNumberPrefab, m_canvas.transform);
        instancedNumber.SetNumber(amount, "");
        Vector3 randomOffset = new Vector3(Random.Range(-m_randomOffsetMult, m_randomOffsetMult),
                                           Random.Range(-m_randomOffsetMult, m_randomOffsetMult),
                                           Random.Range(-m_randomOffsetMult, m_randomOffsetMult));
        instancedNumber.SetPosition(originPosition + randomOffset, m_mainCamera);
    }

    public void SpawnHealNumber(int amount, Vector3 originPosition)
    {
        GPFloatingNumberUI instancedNumber = Instantiate(m_healNumberPrefab, m_canvas.transform);
        instancedNumber.SetNumber(amount, "+");
        Vector3 randomOffset = new Vector3(Random.Range(-m_randomOffsetMult, m_randomOffsetMult),
                                           Random.Range(-m_randomOffsetMult, m_randomOffsetMult),
                                           Random.Range(-m_randomOffsetMult, m_randomOffsetMult));
        instancedNumber.SetPosition(originPosition + randomOffset, m_mainCamera);
    }

}
