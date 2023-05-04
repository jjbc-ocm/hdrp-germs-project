using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance;

    [SerializeField]
    private GameObject mainContent;

    [SerializeField]
    private TMP_Text text;
    
    private bool hasStartTime;

    private double startTime;

    private bool isInitialized;

    private bool isAftermathCalled;

    public double TimeLapse { get => hasStartTime ? PhotonNetwork.Time - startTime : 0; }

    public double ReverseTimeLapse { get => SOManager.Instance.Constants.GameTimer - TimeLapse; }

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!GameManager.Instance.HasStarted) return;

        if (!isInitialized)
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

            isInitialized = true;
        }

        if (!hasStartTime) GetStartTime();

        if (!hasStartTime) return;

        if (GameManager.Instance.IsGameOver() && !isAftermathCalled)
        {
            Debug.Log("GOT IN TIMER");

            //close room for joining players
            PhotonNetwork.CurrentRoom.IsOpen = false;

            PlayerManager.Mine.photonView.RPC("RpcGameOver", RpcTarget.All);

            isAftermathCalled = true;

            return;
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
