using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonetizationManager : Singleton<MonetizationManager>
{



    public async void BuyDummyPart(DummyPartSO dummyPart)
    {
        // TODO: show loading screen

        await APIManager.Instance.PlayerData.Get(); // Ensure that we get correct currency/items values

        if (APIManager.Instance.PlayerData.Gems >= dummyPart.Cost)
        {
            await APIManager.Instance.PlayerData.SubGems(dummyPart.Cost);

            await APIManager.Instance.PlayerData.AddDummyPart(dummyPart);

            // TODO: show player some information that the item was successfully bought
        }
        else
        {
            // TODO: show to player that not enough gems
        }

        // TODO: close loading screen
    }
}
