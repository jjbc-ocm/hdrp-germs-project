using DamageNumbersPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupDebug : MonoBehaviour
{
    public Canvas ui;

    public DamageNumberGUI test;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.IsDebugKey(0))
        {
            var newPopup = test.Spawn(Vector3.zero, 50);

            var pos = Camera.main.WorldToScreenPoint(transform.position);

            newPopup.SetAnchoredPosition(ui.transform, pos);
        }
        
    }
}
