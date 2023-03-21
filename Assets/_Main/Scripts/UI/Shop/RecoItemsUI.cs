using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class RecoItemsUI : UI<RecoItemsUI>
{
    [SerializeField]
    private ShopItemUI prefabItem;

    [SerializeField]
    private GameObject buttonSelectedIndicator;

    /*[SerializeField]
    private ItemSO[] starterItems;*/

    [SerializeField]
    private ItemSO[] consumableItems;

    /* [SerializeField]
     private ItemSO[] offensiveItems;

     [SerializeField]
     private ItemSO[] defensiveItems;

     [SerializeField]
     private ItemSO[] utilityItems;*/

    [SerializeField]
    private Transform transformStarter;

    [SerializeField]
    private Transform transformConsumable;

    [SerializeField]
    private Transform transformOffensive;

    [SerializeField]
    private Transform transformDefensive;

    [SerializeField]
    private Transform transformUtility;

    public bool IsSelected { get; set; }

    protected override void OnRefreshUI()
    {
        buttonSelectedIndicator.SetActive(IsSelected);

        foreach (Transform child in transformStarter)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in transformConsumable)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in transformOffensive)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in transformDefensive)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in transformUtility)
        {
            Destroy(child.gameObject);
        }

        

        foreach (var starterItem in PlayerManager.Mine.Data.IdealStarterItems)
        {
            var item = Instantiate(prefabItem, transformStarter);

            item.RefreshUI((self) =>
            {
                self.Data = starterItem;
            });
        }

        foreach (var consumableItem in consumableItems)
        {
            var item = Instantiate(prefabItem, transformConsumable);

            item.RefreshUI((self) =>
            {
                self.Data = consumableItem;
            });
        }

        foreach (var offensiveItem in PlayerManager.Mine.Data.IdealOffensiveItems)
        {
            var item = Instantiate(prefabItem, transformOffensive);

            item.RefreshUI((self) =>
            {
                self.Data = offensiveItem;
            });
        }

        foreach (var defensiveItem in PlayerManager.Mine.Data.IdealDefensiveItems)
        {
            var item = Instantiate(prefabItem, transformDefensive);

            item.RefreshUI((self) =>
            {
                self.Data = defensiveItem;
            });
        }

        foreach (var utilityItem in PlayerManager.Mine.Data.IdealUtilityItems)
        {
            var item = Instantiate(prefabItem, transformUtility);

            item.RefreshUI((self) =>
            {
                self.Data = utilityItem;
            });
        }
    }
}
