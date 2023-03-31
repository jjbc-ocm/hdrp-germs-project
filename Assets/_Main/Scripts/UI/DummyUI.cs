using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyUI : WindowUI<DummyUI>
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
