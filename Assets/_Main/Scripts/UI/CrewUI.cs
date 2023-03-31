using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewUI : WindowUI<CrewUI>
{
    public void OnHomeClick()
    {
        HomeUI.Instance.Open();

        Close();
    }

    protected override void OnRefreshUI()
    {

    }
}
