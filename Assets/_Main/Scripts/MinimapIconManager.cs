using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapIconManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(90, 0, -90);
    }
}
