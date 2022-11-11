using System.Collections;
using System.Collections.Generic;
using TanksMP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipHUD : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private GameObject mainContent;

    [SerializeField]
    private TMP_Text textName;

    [SerializeField]
    private Image imageMainHealth;

    [SerializeField]
    private Image imageSubHealth;

    [SerializeField]
    private Image imageMainMana;

    [SerializeField]
    private Image imageSubMana;

    public Player Player { get; set; }

    void Update()
    {
        if (Player == null || !IsWithinForOfWar())
        {
            mainContent.SetActive(false);

            return;
        }
        else
        {
            mainContent.SetActive(true);
        }

        transform.position = mainCamera.WorldToScreenPoint(Player.transform.position);

        mainContent.SetActive(transform.position.z > 0);

        textName.text = Player.photonView.GetName();

        imageMainHealth.fillAmount = Player.Health / (float)Player.MaxHealth;

        imageSubHealth.fillAmount += (imageMainHealth.fillAmount - imageSubHealth.fillAmount) / 10f;

        imageMainMana.fillAmount = Player.Mana / (float)Player.MaxMana;

        imageSubMana.fillAmount += (imageMainMana.fillAmount - imageSubMana.fillAmount) / 10f;
    }

    private bool IsWithinForOfWar()
    {
        var selfPos = GameManager.GetInstance().localPlayer.transform.position;

        var otherPos = Player.transform.position;

        return Vector3.Distance(selfPos, otherPos) <= Constants.FOG_OF_WAR_DISTANCE;
    }
}
