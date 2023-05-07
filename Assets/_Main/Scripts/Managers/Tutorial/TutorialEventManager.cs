using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialEventManager : MonoBehaviour
{
    [SerializeField]
    private TutorialEventTriggerType trigger;

    [SerializeField]
    private bool isDestroyOnTrigger;

    [SerializeField]
    private UnityEvent onTrigger;

    #region Unity

    private void Awake()
    {
        if (trigger == TutorialEventTriggerType.OnWaitASecond)
        {
            StartCoroutine(YieldWait(6));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerManager _) && trigger == TutorialEventTriggerType.OnTriggerEnter)
        {
            TutorialManager.Instance.NextEvent();

            onTrigger.Invoke();

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

            onTrigger.Invoke();

            if (isDestroyOnTrigger)
            {
                Destroy(gameObject);
            }
        }
    }

    #endregion

    #region Private

    private IEnumerator YieldWait(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        TutorialManager.Instance.NextEvent();

        onTrigger.Invoke();

        if (isDestroyOnTrigger)
        {
            Destroy(gameObject);
        }
    }

    #endregion
}
