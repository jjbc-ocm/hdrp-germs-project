using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEventManager : MonoBehaviour
{
    [SerializeField]
    private TutorialEventTriggerType trigger;

    [SerializeField]
    private bool isDestroyOnTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerManager _) && trigger == TutorialEventTriggerType.OnTriggerEnter)
        {
            TutorialManager.Instance.NextEvent();

            if (isDestroyOnTrigger)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        if (trigger == TutorialEventTriggerType.OnDestroy)
        {
            TutorialManager.Instance.NextEvent();

            if (isDestroyOnTrigger)
            {
                Destroy(gameObject);
            }
        }
    }
}
