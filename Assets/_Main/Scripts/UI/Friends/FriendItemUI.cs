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
    protected override void OnRefreshUI()
    {

    }
}
