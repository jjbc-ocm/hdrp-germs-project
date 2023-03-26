/*  This file is part of the "Tanks Multiplayer" project by FLOBUK.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;

public class PlayerManager : ActorManager, IPunObservable
{
    public static PlayerManager Mine;

    #region Network Sync

    private Vector3 shipRotation;

    #endregion

    #region Serializable

    [SerializeField]
    private Transform cameraFollow;

    [SerializeField]
    private GPShipDesc data;

    [SerializeField]
    private SkillData attack;

    [SerializeField]
    private SkillData skill;

    #endregion

    private float nextAttackTime;

    private float nextSkillTime;

    private Vector2 moveDir;

    private Vector2 prevMoveDir;

    private bool isRespawning;

    #region Accessors

    public Transform CameraFollow { get => cameraFollow; }

    public GPShipDesc Data { get => data; }

    public SkillData Attack { get => attack; }

    public SkillData Skill { get => skill; }

    public float NextAttackTime { get => nextAttackTime; }

    public float NextSkillTime { get => nextSkillTime; }

    public bool IsRespawning { get => isRespawning; }

    #endregion

    #region Override

    // This method is used on collecting items via collision
    protected override void OnTriggerEnterCalled(Collider col)
    {
        if (!photonView.IsMine) return;

        var key = col.GetComponent<KeyCollectible>();

        var chest = col.GetComponent<ChestCollectible>();

        var collectible = col.GetComponent<Collectible>();

        // Handle collision to key
        if (key != null)
        {
            key.Obtain(this);
        }

        // Handle collision to chest
        else if (chest != null)
        {
            chest.Obtain(this);

            var destination = GameManager.Instance.GetBase(GetTeam()).transform.position;

            GPSManager.Instance.SetDestination(this, destination);
        }

        // Handle collision to normal item
        else if (collectible != null)
        {
            collectible.Obtain(this);
        }
    }

    protected override void OnTriggerExitCalled(Collider col)
    {

    }

    #endregion

    #region Unity

    private void Start()
    {
        gameObject.layer = GetTeam() == PhotonNetwork.LocalPlayer.GetTeam() ?
            LayerMask.NameToLayer(SOManager.Instance.Constants.LayerAlly) :
            LayerMask.NameToLayer(SOManager.Instance.Constants.LayerEnemy);

        Stat.Initialize();

        Aim.Initialize(
            () =>
            {
                if (!ShopManager.Instance.UI.gameObject.activeSelf)
                    ExecuteActionAim(attack, true);
            },
            () =>
            {
                if (!ShopManager.Instance.UI.gameObject.activeSelf)
                    ExecuteActionAim(skill, false);
            },
            (aimPosition, autoTarget) =>
            {
                if (!ShopManager.Instance.UI.gameObject.activeSelf)
                    ExecuteActionInstantly(aimPosition, autoTarget, false);
            },
            () =>
            {
                return Stat.Mana >= attack.MpCost && Time.time > nextAttackTime;
            },
            () =>
            {
                return Stat.Mana >= skill.MpCost && Time.time > nextSkillTime;
            });

        if (!photonView.IsMine || IsBot) return;

        Mine = this;

        GameCameraManager.Instance.SetTarget(cameraFollow);
    }

    private void FixedUpdate()
    {
        if (AftermathUI.Instance.gameObject.activeSelf) return;

        if (!photonView.IsMine || isRespawning) return;

        /* 
            * Update movement
            * Conditions:
            * - UIs that interfere in inputs must be not enabled or in minized state
            * - Is a bot
            */
        if ((!ShopManager.Instance.UI.gameObject.activeSelf && !ChatManager.Instance.UI.IsMaximized) || IsBot)
        {
            if (Input.Move.x == 0 && Input.Move.y == 0)
            {
                moveDir.x += (0 - prevMoveDir.x) * 0.1f;
                moveDir.y += (0 - prevMoveDir.y) * 0.1f;
            }
            else
            {
                moveDir.x += (Input.Move.x - prevMoveDir.x) * 0.1f;
                moveDir.y += (Input.Move.y - prevMoveDir.y) * 0.1f;

                var acceleration = Input.IsSprint ? 2 : 1;

                var moveForce = transform.forward * moveDir.y * Stat.MoveSpeed() * acceleration;

                var moveTorque = transform.up * moveDir.x * Stat.MoveSpeed();

                RigidBody.AddForce(moveForce);

                RigidBody.AddTorque(moveTorque);
            }
        } 

        /* Update rotation */
        shipRotation = new Vector3(
            moveDir.y * -10,
            SoundVisuals.RendererAnchor.transform.localEulerAngles.y,
            moveDir.x * -10);

        SoundVisuals.RendererAnchor.transform.localRotation = Quaternion.Euler(shipRotation);

        /* Cache it because, we need to accumulate the movement force */
        prevMoveDir = moveDir;
    }

    /*private void LateUpdate()
    {
        if (!photonView.IsMine || IsBot) return;

        Cursor.lockState =
            !ShopUI.Instance.gameObject.activeSelf &&
            !ChatUI.Instance.gameObject.activeSelf &&
            !ScoreBoardUI.Instance.gameObject.activeSelf &&
            !AftermathUI.Instance.gameObject.activeSelf &&
            !SettingsUI.Instance.gameObject.activeSelf ?
            CursorLockMode.Locked :
            CursorLockMode.None;
    }*/

    #endregion

    #region Photon

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(shipRotation);
        }
        else
        {
            shipRotation = (Vector3)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void RpcSendChat(string sender, string message, int team, long time)
    {
        ChatManager.Instance.SendChat(sender, message, team, time);
    }

    [PunRPC]
    public void RpcAction(Vector3 fromPosition, Vector3 targetPosition, int targetActorId, bool isAttack)
    {
        var action = isAttack ? attack : skill;

        var forward = targetPosition - fromPosition;

        var rotation = Quaternion.LookRotation(forward);

        var effect = action.IsSpawnOnAim
            ? PoolManager.Instance.Get(action.Effect, targetPosition, rotation)//Instantiate(action.Effect, targetPosition, rotation)
            : PoolManager.Instance.Get(action.Effect, fromPosition, rotation);//Instantiate(action.Effect, fromPosition, rotation);

        var targetActor = PhotonView.Find(targetActorId)?.GetComponent<ActorManager>() ?? null;

        effect.Initialize(this, targetPosition, targetActor);
    }

    [PunRPC]
    public void RpcDestroy(int attackerId)
    {
        var attackerView = PhotonView.Find(attackerId);

        if (PhotonNetwork.IsMasterClient)
        {
            //send player back to the team area, this will get overwritten by the exact position from the client itself later on
            //we just do this to avoid players "popping up" from the position they died and then teleporting to the team area instantly
            //this is manipulating the internal PhotonTransformView cache to update the networkPosition variable
            /*GetComponent<PhotonTransformView>().OnPhotonSerializeView(
                new PhotonStream(
                    false,
                    new object[]
                    {
                        //GameManager.Instance.GetSpawnPosition(photonView.GetTeam()),
                        GameNetworkManager.Instance.SpawnPoints[photonView.GetTeam()].position,
                        Vector3.zero,
                        Quaternion.identity
                    }),
                new PhotonMessageInfo());*/

            ResetPosition(false);
        }

        ToggleFunction(false);

        if (photonView.IsMine)
        {
            if (!IsBot)
            {
                var target = attackerView.TryGetComponent(out PlayerManager player) ? player.cameraFollow : attackerView.transform;

                GameCameraManager.Instance.SetTarget(target);
            }
                
            photonView.RPC("RpcBroadcastKillStatement", RpcTarget.All, attackerView.ViewID, photonView.ViewID);

            StartCoroutine(SpawnRoutine());
        }

        if (onDieEvent != null)
        {
            onDieEvent.Invoke(photonView.ViewID);
        }
    }

    [PunRPC]
    public void RpcBroadcastKillStatement(int attackerId, int defenderId)
    {
        var winner = PhotonView.Find(attackerId).GetComponent<ActorManager>();

        var loser = PhotonView.Find(defenderId).GetComponent<ActorManager>();

        GameManager.Instance.ui.OpenKillStatement(winner, loser);
    }

    [PunRPC]
    public void RpcRevive()
    {
        ToggleFunction(true);

        if (photonView.IsMine)
        {
            ResetPosition(true);
        }
    }

    [PunRPC]
    public void RpcGameOver()
    {
        GameManager.Instance.DisplayGameOver(Array.IndexOf(GameManager.Instance.BattleResults, BattleResultType.Victory));
    }

    [PunRPC]
    public override void RpcDamageHealth(int amount, int attackerId)
    {
        /* Do not damage this ship if respawning */
        if (isRespawning) return;

        Stat.AddHealth(-amount);

        var attacker = PhotonView.Find(attackerId)?.GetComponent<ActorManager>() ?? null;

        //if ((photonView.IsMine && !IsBot) || i)
        if (Mine == this || Mine == attacker)
        {
            PopupManager.Instance.ShowDamage(amount, transform.position);
        }
        
        if (Stat.Health <= 0)
        {
            /* Add score to opponent */
            if (attacker != null)
            {
                var attackerTeam = attacker.GetTeam();

                if (GetTeam() != attackerTeam)
                {
                    //GameManager.Instance.AddScore(ScoreType.Kill, attackerTeam);
                    GameManager.Instance.AddScoreByKill(attackerTeam);

                    GPRewardSystem.m_instance.AddGoldToPlayer(attacker, "Kill");
                }

                /* If the attacker is me, add kill count, then broadcast it */
                if (attacker.photonView.IsMine && attacker.TryGetComponent(out PlayerManager attackerPlayer))
                {
                    attackerPlayer.Stat.AddKill();

                    photonView.RPC("RpcBroadcastKillStatement", RpcTarget.All, attackerId, photonView.ViewID);
                }
            }

            /* Handle collected chest */
            if (Stat.HasChest)
            {
                Stat.SetChest(false);

                PhotonNetwork.InstantiateRoomObject("Chest", transform.position, Quaternion.identity);

                GPSManager.Instance.ClearDestination();
            }
                
            /* Reset stats */
            Stat.SetHealth(Stat.MaxHealth());

            Stat.SetMana(Stat.MaxMana());

            Stat.AddDeath();

            /* Broadcast to all player that this ship is destroyed */
            photonView.RPC("RpcDestroy", RpcTarget.All, attackerId);

            GuideManager.Instance.TryAddShopGuide();
        }
    }

    #endregion

    #region Public

    public void ResetPosition(bool isCameraFollow)
    {
        if (isCameraFollow && !IsBot)
        {
            GameCameraManager.Instance.SetTarget(cameraFollow);
        }

        var spawnPoint = GameManager.Instance.GetBase(GetTeam()).SpawnPoint;

        transform.position = spawnPoint.transform.position;
        transform.rotation = spawnPoint.transform.rotation;

        RigidBody.velocity = Vector3.zero;
        RigidBody.angularVelocity = Vector3.zero;
    }

    #endregion

    #region Private
        
    

    private void ExecuteActionAim(SkillData action, bool isAttack)
    {
        var instantAim =
            action.Aim == AimType.None ? transform.position :
            action.Aim == AimType.WhileExecute ? transform.position + cameraFollow.forward * 999f :
            Vector3.zero;

        if (action.Aim == AimType.None || action.Aim == AimType.WhileExecute)
        {
            ExecuteActionInstantly(instantAim, null, isAttack);
        }
    }

    private void ExecuteActionInstantly(Vector3 aimPosition, ActorManager autoTarget, bool isAttack)
    {
        var action = isAttack ? attack : skill;

        if (isAttack)
        {
            nextAttackTime = Time.time + SOManager.Instance.Constants.MoveSpeedToSecondsRatio / Stat.AttackSpeed();
        }
        else
        {
            nextSkillTime = Time.time + action.Cooldown * (1 - Inventory.StatModifier.BuffCooldown - Status.StatModifier.BuffCooldown);
        }

        Stat.AddMana(-action.MpCost);

        var offset = Vector3.up * 2;

        photonView.RPC(
            "RpcAction",
            RpcTarget.AllViaServer,
            transform.position + offset,
            aimPosition + offset,
            autoTarget?.photonView.ViewID ?? -1,
            isAttack);
    }

    private IEnumerator SpawnRoutine()
    {
        isRespawning = true;

        float targetTime = Time.time + SOManager.Instance.Constants.RespawnTime;

        while (targetTime - Time.time > 0)
        {
            GameManager.Instance.ui.SetSpawnDelay(targetTime - Time.time);

            yield return null;
        }

        GameManager.Instance.ui.DisableDeath();

        isRespawning = false;

        photonView.RPC("RpcRevive", RpcTarget.All);
    }

    private void ToggleFunction(bool toggle)
    {
        SoundVisuals.RendererAnchor.SetActive(toggle);

        foreach (var collider in Colliders)
        {
            collider.enabled = toggle;
        }
    }

    #endregion
}