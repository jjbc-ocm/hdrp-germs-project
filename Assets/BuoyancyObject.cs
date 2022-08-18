using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class BuoyancyObject : MonoBehaviour
{
    public Transform[] floaters;
    public float underWaterDrag = 3f;
    public float underWaterAngularDrag = 1f;
    public float airDrag = 0.0f;
    public float airAngularDrag = 0.05f;
    public float floatingPower = 15f;
    public float offsetY;

    Rigidbody m_Rigidbody;

    int floatersUnderwater;

    bool underwater;

    private float deltaTime;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        deltaTime += Time.deltaTime;

        transform.rotation = Quaternion.Euler(Mathf.Sin(deltaTime) * 3f, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    void FixedUpdate()
    {
        var water = WaterManager.Instance;

        var waterHeight = water ? water.transform.position.y : 0;

        floatersUnderwater = 0;

        for (int i = 0; i < floaters.Length; i++)
        {
            float difference = floaters[i].position.y - waterHeight + offsetY;

            if (difference < 0)
            {
                m_Rigidbody.AddForceAtPosition(Vector3.up * floatingPower * Mathf.Abs(difference), floaters[i].position, ForceMode.Force);

                floatersUnderwater += 1;

                if (!underwater)
                {
                    underwater = true;
                    SwitchState(true);
                }

            }
        }
        
        if (underwater && floatersUnderwater == 0)
        {
            underwater = false;
            SwitchState(false);
        }
    }

    void SwitchState(bool isUnderWater)

    {
        if (isUnderWater)
        {
            m_Rigidbody.drag = underWaterDrag;
            m_Rigidbody.angularDrag = underWaterAngularDrag;
        }
        else
        {
            m_Rigidbody.drag = airDrag;
            m_Rigidbody.angularDrag = airAngularDrag;
        }
    }
}
