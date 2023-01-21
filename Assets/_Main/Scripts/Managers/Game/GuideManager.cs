using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class GuideManager : MonoBehaviour
{
    public static GuideManager Instance;

    [SerializeField]
    private GuideData guideDefault;

    [SerializeField]
    private GuideData guideChest;

    [SerializeField]
    private GuideData guideShop;

    [SerializeField]
    private GuideData guideMonster;

    private List<GuideData> viewedGuides;

    private void Awake()
    {
        Instance = this;

        viewedGuides = new List<GuideData>();
    }

    void Start()
    {
        GameManager.Instance.ui.AddGuideItem(guideDefault);
        //GameManager.Instance.ui.AddGuideItem(guideChest);
        //GameManager.Instance.ui.AddGuideItem(guideShop);
        //GameManager.Instance.ui.AddGuideItem(guideMonster);
    }

    public void TryAddChestGuide()
    {
        if (!viewedGuides.Contains(guideChest))
        {
            GameManager.Instance.ui.AddGuideItem(guideChest);

            viewedGuides.Add(guideChest);
        }
    }

    public void TryAddShopGuide()
    {
        if (!viewedGuides.Contains(guideShop))
        {
            GameManager.Instance.ui.AddGuideItem(guideShop);

            viewedGuides.Add(guideShop);
        }
    }

    public void TryAddMonsterGuide()
    {
        if (!viewedGuides.Contains(guideMonster))
        {
            GameManager.Instance.ui.AddGuideItem(guideMonster);

            viewedGuides.Add(guideMonster);
        }
    }
}
