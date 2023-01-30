
using System.Collections.Generic;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance;

    [SerializeField]
    private ChatUI ui;

    private List<ChatData> chats;

    public ChatUI UI { get => ui; }

    void Awake()
    {
        Instance = this;

        chats = new List<ChatData>();
    }

    void Update()
    {
        if (InputManager.Instance.IsChat)
        {
            ui.IsMaximized = !ui.IsMaximized;
        }
    }

    public void SendChat(string sender, string message, int team, long time)
    {
        chats.Add(new ChatData
        {
            Sender = sender,

            Message = message,

            Team = team,

            Time = time
        });

        ui.RefreshUI((self) =>
        {
            self.Data = chats;
        });
    }
}
