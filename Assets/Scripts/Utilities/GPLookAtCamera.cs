using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPLookAtCamera : MonoBehaviour
{
    public string m_cameraName = "Main Camera";
    GameObject m_cameraRef;

    void Start()
    {
        m_cameraRef = GameObject.Find(m_cameraName);
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = transform.position - m_cameraRef.transform.position;
    }
}
