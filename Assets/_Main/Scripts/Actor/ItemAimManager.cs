using System;
using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;

public class ItemAimManager : MonoBehaviour
{
    [SerializeField]
    private GameObject aimIndicator;

    private bool isAiming;

    private Player player;

    private Action<Vector3> onRelease;

    void Awake()
    {
        player = GetComponent<Player>();
    }

    void Start()
    {
        IndicatorSetActive(false);
    }

    void Update()
    {
        if (!player.photonView.IsMine || player.IsRespawning) return;

        /*if (Input.GetMouseButton(0) && onCanExecuteAttack.Invoke())
        {
            if (!isAiming)
            {
                onAttackPress.Invoke();
            }
        }*/

        /*if (Input.GetKeyDown(KeyCode.Q) && onCanExecuteSkill.Invoke())
        {
            isAiming = true;

            onAimSkillPress.Invoke();
        }*/

        if (Input.GetMouseButton(0) && isAiming)
        {
            isAiming = false;

            if (aimIndicator.activeSelf)
            {
                onRelease.Invoke(aimIndicator.transform.position);
                //onAimSkillRelease.Invoke(aimIndicator.transform.position, aimAutoTarget);
            }
        }

        if (isAiming)
        {
            var action = player.Skill;

            var ray = player.CamFollow.Cam.ScreenPointToRay(Input.mousePosition);

            //var layerNames = action.Aim == AimType.Water ? new string[] { "Water" } : new string[] { "Ship", "Monster" };

            var camDistanceToShip = Vector3.Distance(player.transform.position, player.CamFollow.Cam.transform.position);

            if (Physics.Raycast(ray, out RaycastHit hit, action.Range + camDistanceToShip, LayerMask.GetMask("Water")))
            {
                /*if (action.Aim == AimType.Water ||
                    action.Aim == AimType.AnyShip ||
                    (action.Aim == AimType.EnemyShip && IsEnemyShip(hit)) ||
                    (action.Aim == AimType.AllyShip && !IsEnemyShip(hit)))
                {*/
                    IndicatorSetActive(true);

                    aimIndicator.transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);

                    /*if (action.Aim == AimType.EnemyShip ||
                        action.Aim == AimType.AllyShip ||
                        action.Aim == AimType.AnyShip)
                    {
                        aimAutoTarget = hit.transform.GetComponent<ActorManager>();
                    }*/
                //}
            }
            else
            {
                IndicatorSetActive(false);
            }
        }
        else
        {
            IndicatorSetActive(false);
        }
    }

    public void Aim(Action<Vector3> onRelease)
    {
        isAiming = true;

        this.onRelease = onRelease;
    }

    private void IndicatorSetActive(bool value)
    {
        aimIndicator.SetActive(value);
    }
}
