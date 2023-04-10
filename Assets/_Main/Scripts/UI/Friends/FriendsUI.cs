using DG.Tweening;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendsUI : ListViewUI<FriendItemUI, FriendsUI>
{
    [SerializeField]
    private GameObject handle;

    private IEnumerable<Friend> Data { get; set; }

    #region Unity

    private void Start()
    {
        StartCoroutine(YieldRefreshFriendList());
    }

    #endregion

    #region Public

    public void OnMaximizeClick()
    {
        (transform as RectTransform).DOAnchorPosX(0, 0.25f, true);

        handle.SetActive(false);
    }

    public void OnMinimizeClick()
    {
        (transform as RectTransform).DOAnchorPosX(1200 * transform.localScale.x, 0.25f, true);

        handle.SetActive(true);
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
                self.Data = SteamFriends.GetFriends();//APIManager.Instance.GetFriends();
            });
        }
    }

    #endregion
}
