using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendsUI : ListViewUI<FriendItemUI, FriendsUI>
{
    private List<FriendInfo> Data { get; set; }

    private void Start()
    {
        StartCoroutine(YieldRefreshFriendList());
    }

    protected override void OnRefreshUI()
    {
        DeleteItems();

        RefreshItems(Data, (item, data) =>
        {
            item.Data = data;
        });
    }

    private IEnumerator YieldRefreshFriendList()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            RefreshUI((self) =>
            {
                self.Data = APIManager.Instance.GetFriends();
            });
        }
    }
}
