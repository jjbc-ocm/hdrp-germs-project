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

    private Action<Vector3, ActorManager> onAimSkillRelease;

    private Func<bool> onCanExecuteAttack;

    private Func<bool> onCanExecuteSkill;

    private bool isAiming;

    private ActorManager aimAutoTarget;

    private Player player;

    void Awake()
    {
        player = GetComponent<Player>();
    }

    void Start()
    {
        IndicatorSetActive(false);
    }

    public void Initialize(Action onAttackPress, Action onAimSkillPress, Action<Vector3, ActorManager> onAimSkillRelease, Func<bool> onCanExecuteAttack, Func<bool> onCanExecuteSkill)
    {
        this.onAttackPress = onAttackPress;

        this.onAimSkillPress = onAimSkillPress;

        this.onAimSkillRelease = onAimSkillRelease;

        this.onCanExecuteAttack = onCanExecuteAttack;

        this.onCanExecuteSkill = onCanExecuteSkill;
    }

    void Update()
    {
        if (!player.photonView.IsMine || player.IsRespawning) return;

        if (Input.GetMouseButton(0) && onCanExecuteAttack.Invoke())
        {
            if (!isAiming)
            {
                onAttackPress.Invoke();
            }
        }

        if (Input.GetMouseButtonDown(1) && onCanExecuteSkill.Invoke())
        {
            isAiming = true;

            onAimSkillPress.Invoke();
        }

        if (Input.GetMouseButtonUp(1) && isAiming)
        {
            isAiming = false;

            if (aimIndicator.activeSelf)
            {
                onAimSkillRelease.Invoke(aimIndicator.transform.position, aimAutoTarget);
            }
        }

        if (isAiming)
        {
            var action = player.Skill;

            var ray = player.CamFollow.Cam.ScreenPointToRay(Input.mousePosition);

            var layerNames = action.Aim == AimType.Water ? new string[] { "Water" } : new string[] { "Ship", "Monster" };

            var camDistanceToShip = Vector3.Distance(player.transform.position, player.CamFollow.Cam.transform.position);

            if (Physics.Raycast(ray, out RaycastHit hit, action.Range + camDistanceToShip, LayerMask.GetMask(layerNames)))
            {
                if (action.Aim == AimType.Water ||
                    action.Aim == AimType.AnyShip ||
                    (action.Aim == AimType.EnemyShip && IsEnemyShip(hit)) ||
                    (action.Aim == AimType.AllyShip && !IsEnemyShip(hit)))
                {
                    IndicatorSetActive(true);

                    aimIndicator.transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);

                    if (aimTrailIndicator != null)
                    {
                        var targetEuler = Quaternion.LookRotation(hit.point - transform.position).eulerAngles;

                        aimTrailIndicator.transform.eulerAngles = new Vector3(0, targetEuler.y, 0);
                    }

                    if (action.Aim == AimType.EnemyShip ||
                        action.Aim == AimType.AllyShip ||
                        action.Aim == AimType.AnyShip)
                    {
                        aimAutoTarget = hit.transform.GetComponent<ActorManager>();
                    }
                }
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

    private bool IsEnemyShip(RaycastHit hit)
    {
        var hitPlayer = hit.transform.GetComponent<ActorManager>();

        return !hitPlayer || (hitPlayer && hitPlayer.photonView.GetTeam() != player.photonView.GetTeam());
    }

    private void IndicatorSetActive(bool value)
    {
        aimIndicator.SetActive(value);

        if (aimTrailIndicator != null)
        {
            aimTrailIndicator.SetActive(value);
        }
    }
}
