using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoItemsUI : UI<RecoItemsUI>
{
    [SerializeField]
    private ShopItemUI prefabItem;

    [SerializeField]
    private GameObject buttonSelectedIndicator;

    [SerializeField]
    private ItemData[] starterItems;

    [SerializeField]
    private ItemData[] consumableItems;

    [SerializeField]
    private ItemData[] offensiveItems;

    [SerializeField]
    private ItemData[] defensiveItems;

    [SerializeField]
    private ItemData[] utilityItems;

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
            //Destroy(child.gameObject);
        }

        foreach (Transform child in transformConsumable)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in transformOffensive)
        {
            //Destroy(child.gameObject);
        }

        foreach (Transform child in transformDefensive)
        {
            //Destroy(child.gameObject);
        }

        foreach (Transform child in transformUtility)
        {
            //Destroy(child.gameObject);
        }

        foreach (var starterItem in starterItems)
        {

        }

        foreach (var consumableItem in consumableItems)
        {
            var item = Instantiate(prefabItem, transformConsumable);

            item.RefreshUI((self) =>
            {
                self.Data = consumableItem;
            });
        }

        foreach (var offensiveItem in offensiveItems)
        {

        }

        foreach (var defensiveItem in defensiveItems)
        {

        }

        foreach (var utilityItem in utilityItems)
        {

        }
    }
}
