using System;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class AimManager : MonoBehaviour
{
    [SerializeField]
    private GameObject aimIndicator;

    [SerializeField]
    private GameObject aimTrailIndicator;

    private Action onAttackPress;

    private Action onAimSkillPress;

    private Action<Vector3, Player> onAimSkillRelease;

    private bool isAiming;

    private Player aimAutoTarget;

    private Player player;

    public bool IsAiming { get => isAiming; set => isAiming = value; }

    void Awake()
    {
        player = GetComponent<Player>();
    }

    void Start()
    {
        aimIndicator.SetActive(false);

        if (aimTrailIndicator != null)
        {
            aimTrailIndicator.SetActive(false);
        }
    }

    public void Initialize(Action onAttackPress, Action onAimSkillPress, Action<Vector3, Player> onAimSkillRelease)
    {
        this.onAttackPress = onAttackPress;

        this.onAimSkillPress = onAimSkillPress;

        this.onAimSkillRelease = onAimSkillRelease;
    }

    void Update()
    {
        if (!player.photonView.IsMine) return;

        if (Input.GetMouseButton(0))
        {
            if (!isAiming)
            {
                onAttackPress.Invoke();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            onAimSkillPress.Invoke();
        }

        if (Input.GetMouseButtonUp(1) && isAiming)
        {
            onAimSkillRelease.Invoke(aimIndicator.transform.position, aimAutoTarget);
        }

        if (isAiming)
        {
            var action = player.Skill;

            var ray = player.CamFollow.Cam.ScreenPointToRay(Input.mousePosition);

            var layerName = action.Aim == AimType.Water ? "Water" : "Ship";

            if (Physics.Raycast(ray, out RaycastHit hit, action.Range, LayerMask.GetMask(layerName)))
            {
                if (action.Aim == AimType.Water ||
                    action.Aim == AimType.AnyShip ||
                    (action.Aim == AimType.EnemyShip && IsEnemyShip(hit)) ||
                    (action.Aim == AimType.AllyShip && !IsEnemyShip(hit)))
                {
                    aimIndicator.SetActive(true);

                    aimIndicator.transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);

                    if (aimTrailIndicator != null)
                    {
                        var targetEuler = Quaternion.LookRotation(hit.point - transform.position).eulerAngles;

                        aimTrailIndicator.SetActive(true);

                        aimTrailIndicator.transform.eulerAngles = new Vector3(0, targetEuler.y, 0);
                    }

                    if (action.Aim == AimType.EnemyShip ||
                        action.Aim == AimType.AllyShip ||
                        action.Aim == AimType.AnyShip)
                    {
                        aimAutoTarget = hit.transform.GetComponent<Player>();
                    }
                }
            }
            else
            {
                aimIndicator.SetActive(false);

                if (aimTrailIndicator != null)
                {
                    aimTrailIndicator.SetActive(false);
                }
            }
        }
        else
        {
            aimIndicator.SetActive(false);

            if (aimTrailIndicator != null)
            {
                aimTrailIndicator.SetActive(false);
            }
        }
    }

    private bool IsEnemyShip(RaycastHit hit)
    {
        var hitPlayer = hit.transform.GetComponent<Player>();

        return !hitPlayer || (hitPlayer && hitPlayer.photonView.GetTeam() != player.photonView.GetTeam());
    }
}
