using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class TeamIndicatorManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] teamIndicators;

    private Player player;

    void Start()
    {
        player = GetComponent<Player>();

        teamIndicators[player.photonView.GetTeam()].SetActive(true);
    }
}
