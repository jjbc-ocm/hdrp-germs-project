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

    [SerializeField]
    private GameObject aimRangeIndicator;

    private Action onAttackPress;

    private Action onAimSkillPress;

    private Action<Vector3, ActorManager> onAimSkillRelease;

    private Func<bool> onCanExecuteAttack;

    private Func<bool> onCanExecuteSkill;

    private bool isAiming;

    private ActorManager aimAutoTarget;

    private Player player;

    public bool IsAiming { get => isAiming; }

    #region Unity

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        IndicatorSetActive(false, false);
    }

    private void Update()
    {
        if (AftermathUI.Instance.gameObject.activeSelf) return;

        if (ShopManager.Instance.UI.gameObject.activeSelf) return;

        if (!player.photonView.IsMine || player.IsRespawning) return;

        if (player.Input.IsAttack && onCanExecuteAttack.Invoke())
        {
            if (!isAiming)
            {
                onAttackPress.Invoke();
            }
        }

        if (player.Input.IsAim && onCanExecuteSkill.Invoke())
        {
            isAiming = true;

            onAimSkillPress.Invoke();
        }
        
        /* Executed when player release the aim, which will execute the skill */
        if (!player.Input.IsAim && isAiming)
        {
            isAiming = false;

            if (aimAutoTarget || !IsAimAutoTarget(player.Skill))
            {
                onAimSkillRelease.Invoke(aimIndicator.transform.position, aimAutoTarget);
            }
        }

        if (player.Input.IsAimCancel)
        {
            isAiming = false;
        }

        if (isAiming)
        {
            var constants = SOManager.Instance.Constants;

            var action = player.Skill;

            var ray = player.IsBot
                ? player.Bot.GetRay()
                : GameCameraManager.Instance.MainCamera.ScreenPointToRay(player.Input.Look);

            var layerNames =
                action.Aim == AimType.Water ? new string[] { constants.LayerWater } :
                action.Aim == AimType.AllyShip ? new string[] { constants.LayerAlly } :
                action.Aim == AimType.EnemyShip ? new string[] { constants.LayerEnemy, constants.LayerMonster } :
                new string[] { constants.LayerAlly, constants.LayerEnemy, constants.LayerMonster };

            var camDistanceToShip = Vector3.Distance(player.transform.position, GameCameraManager.Instance.MainCamera.transform.position);

            var maxDistance = aimTrailIndicator == null
                ? action.Range + camDistanceToShip
                : float.MaxValue;

            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, LayerMask.GetMask(layerNames)))
            {
                if (action.Aim == AimType.Water ||
                    action.Aim == AimType.AnyShip ||
                    (action.Aim == AimType.EnemyShip && IsEnemyShip(hit)) ||
                    (action.Aim == AimType.AllyShip && !IsEnemyShip(hit)))
                {
                    IndicatorSetActive(true, true);

                    aimIndicator.transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);

                    if (aimTrailIndicator != null)
                    {
                        var targetEuler = Quaternion.LookRotation(hit.point - transform.position).eulerAngles;

                        aimTrailIndicator.transform.eulerAngles = new Vector3(0, targetEuler.y, 0);
                    }

                    if (IsAimAutoTarget(action))
                    {
                        aimAutoTarget = hit.transform.GetComponent<ActorManager>();
                    }
                }
            }
            else
            {
                IndicatorSetActive(false, true);

                aimAutoTarget = null;
            }
        }
        else
        {
            IndicatorSetActive(false, false);
        }
    }

    #endregion

    #region Public

    public void Initialize(
        Action onAttackPress,
        Action onAimSkillPress,
        Action<Vector3, ActorManager> onAimSkillRelease,
        Func<bool> onCanExecuteAttack,
        Func<bool> onCanExecuteSkill)
    {
        this.onAttackPress = onAttackPress;

        this.onAimSkillPress = onAimSkillPress;

        this.onAimSkillRelease = onAimSkillRelease;

        this.onCanExecuteAttack = onCanExecuteAttack;

        this.onCanExecuteSkill = onCanExecuteSkill;
    }

    #endregion

    private bool IsAimAutoTarget(SkillData action)
    {
        return
            action.Aim == AimType.EnemyShip ||
            action.Aim == AimType.AllyShip ||
            action.Aim == AimType.AnyShip;
    }

    private bool IsEnemyShip(RaycastHit hit)
    {
        var hitPlayer = hit.transform.GetComponent<ActorManager>();

        return !hitPlayer || (hitPlayer && hitPlayer.GetTeam() != player.GetTeam()) || hitPlayer.IsMonster;
    }

    private void IndicatorSetActive(bool aim, bool range)
    {
        var isMine = player.photonView.IsMine && !player.IsBot;

        aimIndicator.SetActive(aim && isMine);

        aimRangeIndicator.SetActive(range && isMine);

        if (aimTrailIndicator != null)
        {
            aimTrailIndicator.SetActive(aim && isMine);
        }
    }
}
