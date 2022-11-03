using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPGParentOnStart : MonoBehaviour
{
    public Transform m_parent;
    public bool m_setToOrigin = true;

    // Start is called before the first frame update
    void Start()
    {
        transform.SetParent(m_parent);
        if (m_setToOrigin)
        {
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
        }
    }
}
