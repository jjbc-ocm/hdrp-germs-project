using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class MinimapIconManager : MonoBehaviour
{
    void Update()
    {
        transform.eulerAngles = new Vector3(90, 0, Player.Mine.GetTeam() == 0 ? 270 : 90);
    }
}
