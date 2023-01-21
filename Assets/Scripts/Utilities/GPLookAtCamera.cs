using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPLookAtCamera : MonoBehaviour
{
    [Tooltip("Name of the game object of the camera that you want this componnet to look at")]
    public string m_cameraName = "Main Camera";
    GameObject m_cameraRef;

    void Start()
    {
        //Find camera in scene by name.
        m_cameraRef = GameObject.Find(m_cameraName);
    }

    // Update is called once per frame
    void Update()
    {
        //Always look at the referenced camera.
        transform.forward = transform.position - m_cameraRef.transform.position;
    }
}
