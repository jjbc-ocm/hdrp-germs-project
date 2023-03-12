using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreparationManager : MonoBehaviour
{
    private void Start()
    {
        PreparationUI.Instance.RefreshUI();
    }
}
