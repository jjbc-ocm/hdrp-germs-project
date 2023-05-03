using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    #region Instance Variables

    [SerializeField]
    private TutorialInfo[] infos;

    private int eventIndex;

    #endregion

    #region Unity

    private void Awake()
    {
        NextEvent();
    }

    private void Update()
    {
        
    }

    #endregion

    #region Public

    public void NextEvent()
    {
        eventIndex += 1;

        foreach (var info in infos)
        {
            info.Prefab.SetActive(eventIndex >= info.EventIndexMin && eventIndex <= info.EventIndexMax);
        }
    }

    #endregion
}
