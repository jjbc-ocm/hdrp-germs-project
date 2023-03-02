using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatData
{
    private string sender;

    private string message;

    private int team;

    private long time;

    public string Sender { get => sender; set => sender = value; }

    public string Message { get => message; set => message = value; }

    public int Team { get => team; set => team = value; }
    
    public long Time { get => time; set => time = value; }
}
