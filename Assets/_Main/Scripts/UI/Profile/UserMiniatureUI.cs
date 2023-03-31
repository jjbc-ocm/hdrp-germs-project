using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserMiniatureUI : UI<UserMiniatureUI>
{
    public void OnClick()
    {
        ProfileUI.Instance.Open((self) =>
        {
            self.Data = APIManager.Instance.PlayerData;
        });
    }

    protected override void OnRefreshUI()
    {

    }
}
