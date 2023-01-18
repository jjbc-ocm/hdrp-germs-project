using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class GuideManager : MonoBehaviour
{
    [SerializeField]
    private GuideData guideDefault;

    [SerializeField]
    private GuideData guideChest;

    [SerializeField]
    private GuideData guideShop;

    [SerializeField]
    private GuideData guideMonster;

    void Start()
    {
        GameManager.Instance.ui.AddGuideItem(guideDefault);
        GameManager.Instance.ui.AddGuideItem(guideChest);
        GameManager.Instance.ui.AddGuideItem(guideShop);
        GameManager.Instance.ui.AddGuideItem(guideMonster);
    }

    void Update()
    {
        
    }
}
