using System.Collections;
using System.Collections.Generic;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private GameObject mainContent;

    [SerializeField]
    private TMP_Text textName;

    [SerializeField]
    private Image imageMainProgress;

    [SerializeField]
    private Image imageSubProgress;

    public Player Player { get; set; }

    void Update()
    {
        if (Player == null)
        {
            mainContent.SetActive(Player != null);

            return;
        }

        transform.position = mainCamera.WorldToScreenPoint(Player.transform.position);

        mainContent.SetActive(transform.position.z > 0);

        textName.text = Player.GetView().GetName();

        imageMainProgress.fillAmount = Player.GetView().GetHealth() / (float)Player.maxHealth;

        imageSubProgress.fillAmount += (imageMainProgress.fillAmount - imageSubProgress.fillAmount) / 10f;
    }
}
