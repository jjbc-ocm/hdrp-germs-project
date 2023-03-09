using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FriendItemUI : UI<FriendItemUI>
{
    [SerializeField]
    private TMP_Text textName;

    [SerializeField]
    private TMP_Text textStatus;

    [SerializeField]
    private GameObject objectStatus;

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

    #endregion
}
