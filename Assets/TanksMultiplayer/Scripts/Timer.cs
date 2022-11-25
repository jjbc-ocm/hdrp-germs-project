using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public static Timer Instance;

    [SerializeField]
    private GameObject mainContent;

    [SerializeField]
    private TMP_Text text;
    
    private bool hasStartTime;

    private double startTime;

    public double TimeLapse { get => hasStartTime ? PhotonNetwork.Time - startTime : 0; }

    public double ReverseTimeLapse { get => GameManager.GetInstance().gameTimer - TimeLapse; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startTime = PhotonNetwork.Time;

            hasStartTime = true;

            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                { "startTime", startTime }
            });
        }
        else
        {
            GetStartTime();
        }
    }

    void Update()
    {
        if (!hasStartTime) GetStartTime();

        if (!hasStartTime) return;

        if (GameManager.GetInstance().IsGameOver())
        {
            int[] score = PhotonNetwork.CurrentRoom.GetScore();

            //close room for joining players
            PhotonNetwork.CurrentRoom.IsOpen = false;
            //tell all clients the winning team
            Player.Mine.photonView.RPC("RpcGameOver", RpcTarget.All, (byte)(score[0] > score[1] ? 0 : 1));
            return;
        }

        /* Update UI */
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("mode", out object mode))
        {
            var intMode = Convert.ToInt32(mode);

            var enumMode = (GameMode)intMode;

            mainContent.SetActive(enumMode == GameMode.Survival);
        }

        var timeSpan = TimeSpan.FromSeconds(ReverseTimeLapse);

        var timeText = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);

        text.text = timeText;
    }

    void GetStartTime()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("startTime", out object value))
        {
            startTime = Convert.ToDouble(value);

            hasStartTime = true;
        }
        
    }

}
