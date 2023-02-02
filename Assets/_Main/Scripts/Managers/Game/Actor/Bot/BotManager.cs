using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotManager : MonoBehaviour
{
    #region Unity

    private void Start()
    {
        StartCoroutine(YieldDecisionMaking());
    }

    #endregion

    #region Private

    private IEnumerator YieldDecisionMaking()
    {
        yield return new WaitForSeconds(Random.value);

        while (true)
        {
            yield return new WaitForSeconds(1);

            DecisionMaking();
        }
    }

    private void DecisionMaking()
    {
        // Create list of possible decisions I can make
        // 1. get everything in my FOV
        var entities = transform.GetEntityInRange(Constants.FOG_OF_WAR_DISTANCE);

        foreach (var entity in entities)
        {

        }

        // Add weight to it based on conditions

        // Compare which decision is best

        // The best decision will be executed
    }

    #endregion
}

public class DecisionThread
{
    private void DecisionMaking()
    {
        // Create list of possible decisions I can make

        // Add weight to it based on conditions

        // Compare which decision is best

        // The best decision will be executed
    }
}