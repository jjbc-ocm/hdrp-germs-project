using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AftermathUI : UI<AftermathUI>
{
    [SerializeField]
    private GameObject indicatorVictory;

    [SerializeField]
    private GameObject indicatorDefeat;

    public bool IsVictory { get; set; }

    protected override void OnRefreshUI()
    {

    }

    public void OnClick()
    {
        SceneManager.LoadScene(Constants.MENU_SCENE_NAME);
    }
}
