using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text textMessage;

    public string Text { get; set; }

    void Update()
    {
        textMessage.text = Text;
    }
}
