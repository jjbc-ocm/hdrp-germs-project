using System.Collections;
using System.Collections.Generic;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AftermathUI : UI<AftermathUI>
{
    [SerializeField]
    private TMP_Text textMessage;

    [SerializeField]
    private GameObject indicatorVictory;

    [SerializeField]
    private GameObject indicatorDefeat;

    [SerializeField]
    private GameObject indicatorDraw;

    public Team WinnerTeam { get; set; }

    public BattleResultType BattleResult { get; set; }

    public bool IsMessageDone { get; set; }

    void OnEnable()
    {
        StartCoroutine(YieldProceed());
    }

    protected override void OnRefreshUI()
    {
        textMessage.gameObject.SetActive(!IsMessageDone);

        if (!IsMessageDone)
        {
            textMessage.text = WinnerTeam != null
                ? "TEAM <color=#" + ColorUtility.ToHtmlStringRGB(WinnerTeam.material.color) + ">" + WinnerTeam.name + "</color> WINS!"
                : "DRAW!";
        }
        else
        {
            indicatorVictory.SetActive(BattleResult == BattleResultType.Victory);

            indicatorDefeat.SetActive(BattleResult == BattleResultType.Defeat);

            indicatorDraw.SetActive(BattleResult == BattleResultType.Draw);
        }
    }

    public void OnClick()
    {
        SceneManager.LoadScene(Constants.MENU_SCENE_NAME);
    }



    private IEnumerator YieldProceed()
    {
        yield return new WaitForSeconds(3);

        RefreshUI((self) =>
        {
            self.IsMessageDone = true;
        });
    }
}
