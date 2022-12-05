using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text textMessage;

    [SerializeField]
    private Slider sliderProgress;

    public string Text { get; set; }

    public float Progress { get; set; }

    void Update()
    {
        textMessage.text = Text;

        sliderProgress.value = Progress;
    }
}
