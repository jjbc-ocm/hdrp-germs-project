
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatManager : Singleton<ChatManager>
{
    [SerializeField]
    private ChatUI ui;

    private List<ChatData> chats;

    public ChatUI UI { get => ui; }

    #region Unity

    private void Start()
    {
        chats = new List<ChatData>();
    }

    private void Update()
    {
        if (InputManager.Instance.IsChat)
        {
            ui.RefreshUI((self) =>
            {
                self.Data = chats;

                if (string.IsNullOrEmpty(self.InputMessage.text))
                {
                    self.IsMaximized = !self.IsMaximized;
                }
            });
        }
    }

    #endregion

    public void SendChat(string sender, string message, int team, long time)
    {
        if (string.IsNullOrEmpty(message)) return;

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

            /*self.InputMessage.text = "";*/
            StartCoroutine(YieldResetInputMessage());
        });
    }

    private IEnumerator YieldResetInputMessage()
    {
        yield return new WaitForEndOfFrame();

        ui.InputMessage.text = "";
    }
}
