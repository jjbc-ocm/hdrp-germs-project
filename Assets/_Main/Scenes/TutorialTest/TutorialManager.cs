using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    #region Instance Variables

    [SerializeField]
    private TutorialInfo[] infos;

    private int part;

    #endregion

    #region Unity

    private void Update()
    {
        
    }

    #endregion

    #region Public

    public void NextPart()
    {
        part += 1;

    }

    #endregion
}
