using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendItemUI : UI<FriendItemUI>
{
    [SerializeField]
    private Sprite[] spriteStatuses;

    [SerializeField]
    private TMP_Text textName;

    [SerializeField]
    private TMP_Text textStatus;

    [SerializeField]
    private Image imageStatus;

    public Friend Data { get; set; }

    #region Public

    public void InviteButtonClick()
    {
        //APIManager.Instance.InviteFriend(Data.SteamID);
    }

    #endregion

    #region Protected

    protected override void OnRefreshUI()
    {
        textName.text = Data.Name;

        textStatus.text = GetStatusAsString();

        imageStatus.sprite = GetStatusAsSprite();
    }

    #endregion

    #region Private

    private string GetStatusAsString()
    {
        if (Data.IsOnline)
            return "Online";

        if (Data.IsBusy)
            return "Busy";

        if (Data.IsAway)
            return "Away";

        return "Offline";
    }

    private Sprite GetStatusAsSprite()
    {
        if (Data.IsOnline)
            return spriteStatuses[1];

        return spriteStatuses[0];
    }

    #endregion
}
