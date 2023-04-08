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

    public FriendInfo Data { get; set; }

    #region Public

    public void InviteButtonClick()
    {
        APIManager.Instance.InviteFriend(Data.SteamID);
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
        if (Data.Status == EPersonaState.k_EPersonaStateOffline)
            return "Offline";

        if (Data.Status == EPersonaState.k_EPersonaStateBusy)
            return "Busy";

        if (Data.Status == EPersonaState.k_EPersonaStateAway ||
            Data.Status == EPersonaState.k_EPersonaStateSnooze)
            return "Away";

        return "Online";
    }

    private Sprite GetStatusAsSprite()
    {
        if (Data.Status == EPersonaState.k_EPersonaStateOffline)
            return spriteStatuses[0];

        if (Data.Status == EPersonaState.k_EPersonaStateBusy)
            return spriteStatuses[0];

        if (Data.Status == EPersonaState.k_EPersonaStateAway ||
            Data.Status == EPersonaState.k_EPersonaStateSnooze)
            return spriteStatuses[0];

        return spriteStatuses[1];
    }

    #endregion
}
