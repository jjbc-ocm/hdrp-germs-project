using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendsUI : ListViewUI<FriendItemUI, FriendsUI>
{
    private List<FriendInfo> Data { get; set; }

    private bool isMaximized;

    #region Unity

    private void Start()
    {
        StartCoroutine(YieldRefreshFriendList());
    }

    #endregion

    #region Public

    public void OnDockerClick()
    {
        isMaximized = !isMaximized;

        var direction = isMaximized ? 0 : 1;

        (transform as RectTransform).DOAnchorPosX(1200 * transform.localScale.x * direction, 0.25f, true);
    }

    #endregion

    #region Override

    protected override void OnRefreshUI()
    {
        DeleteItems();

        RefreshItems(Data, (item, data) =>
        {
            item.Data = data;
        });
    }

    #endregion

    #region Private

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

    #endregion
}
