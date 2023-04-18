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
        if (key)
        {
            key.photonView.RPC("RpcObtain", photonView.Owner, photonView.ViewID);
        }

        // Handle collision to chest
        else if (chest)
        {
            chest.photonView.RPC("RpcObtain", photonView.Owner, photonView.ViewID);
        }

        // Handle collision to normal item
        else if (collectible)
        {
            collectible.photonView.RPC("RpcObtain", photonView.Owner, photonView.ViewID);
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
    public void RpcActionActor(Vector3 fromPosition, Vector3 targetPosition, int targetActorId, bool isAttack)
    {
        var action = isAttack ? attack : skill;

        var forward = targetPosition - fromPosition;

        var rotation = Quaternion.LookRotation(forward);

        var effect = action.IsSpawnOnAim
            ? Instantiate(action.Effect, targetPosition, rotation)//PoolManager.Instance.Get(action.Effect, targetPosition, rotation)//
            : Instantiate(action.Effect, fromPosition, rotation);//PoolManager.Instance.Get(action.Effect, fromPosition, rotation);//

        var targetActor = PhotonView.Find(targetActorId)?.GetComponent<ActorManager>() ?? null;

        effect.Initialize(this, targetPosition, targetActor);
    }

    [PunRPC]
    // This method must received by all
    // This must use when disabling/enabling the object back during respawning process
    public void RpcSetAvailability(bool isAvailable)
    {
        SoundVisuals.RendererAnchor.SetActive(isAvailable);

        foreach (var collider in Colliders)
        {
            collider.enabled = isAvailable;
        }
    }

    /*[PunRPC]
    // This method must received by all
    // Similar to RpcSetAvailability, but this is to show in UI instead
    public void RpcBroadcastKillStatement(int attackerId, int defenderId)
    {
        var winner = PhotonView.Find(attackerId).GetComponent<ActorManager>();

        var loser = PhotonView.Find(defenderId).GetComponent<ActorManager>();

        GameManager.Instance.ui.OpenKillStatement(winner, loser);
    }*/

    [PunRPC]
    public void RpcGameOver()
    {
        GameManager.Instance.DisplayGameOver(Array.IndexOf(GameManager.Instance.BattleResults, BattleResultType.Victory));
    }

    [PunRPC]
    // This method must only received by the player who will get damaged
    public override void RpcDamageHealth(int amount, int attackerId)
    {
        // Do not damage this ship if respawning
        if (isRespawning) return;

        Stat.AddHealth(-amount);
        
        // Additional logic must be executed if this ship is destroyed
        if (Stat.Health <= 0)
        {
            var attacker = PhotonView.Find(attackerId)?.GetComponent<ActorManager>() ?? null;

            // Add score to opponent
            if (attacker != null)
            {
                var attackerTeam = attacker.GetTeam();

                if (GetTeam() != attackerTeam)
                {
                    GameManager.Instance.AddScoreByKill(attackerTeam);

                    GPRewardSystem.m_instance.AddGoldToPlayer(attacker, "Kill");
                }

                // Add kill counter to the attacker. But since I cannot directly do it since that property is owned by attacker,
                // I'll do a RPC call to tell to that attacker that it got a kill counter instead.
                attacker.photonView.RPC("RpcAddKill", attacker.photonView.Owner);
            }

            // Drop chest if there is
            if (Stat.HasChest())
            {
                var team = 
                    Stat.HasChest(0) ? 0 : 
                    Stat.HasChest(1) ? 1 : 
                    -1;

                // TODO: do not hard-code
                var prefabName = 
                    team == 0 ? "Chest - Red" :
                    team == 1 ? "Chest - Blue" :
                    "???";

                PhotonNetwork.InstantiateRoomObject(prefabName, transform.position, Quaternion.identity);

                Stat.SetChest(false);
            }

            // Drop key if there is
            if (Stat.HasKey)
            {
                PhotonNetwork.InstantiateRoomObject("Key", transform.position, Quaternion.identity);

                Stat.SetKey(false);
            }
                
            // Reset anything that needed to be reset
            Stat.SetHealth(Stat.MaxHealth());

            Stat.SetMana(Stat.MaxMana());

            Stat.AddDeath();

            ResetAndDestroy();

            // Broadcast to all players that this ship has been destroyed
            photonView.RPC("RpcSetAvailability", RpcTarget.All, false);

            GameManager.Instance.ui.photonView.RPC("RpcKillStatement", RpcTarget.All, attackerId, photonView.ViewID);

            GuideManager.Instance.TryAddShopGuide();
        }
    }

    #endregion

    #region Private

    private void ResetPosition(bool isCameraFollow)
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
            "RpcActionActor",
            RpcTarget.AllViaServer,
            transform.position + offset,
            aimPosition + offset,
            autoTarget?.photonView.ViewID ?? -1,
            isAttack);
    }

    private void ResetAndDestroy()
    {
        ResetPosition(false);

        if (!IsBot)
        {
            GameManager.Instance.AimCameraToNextPlayer();
        }

        StartCoroutine(YieldSpawn());
    }

    private IEnumerator YieldSpawn()
    {
        isRespawning = true;

        float targetTime = Time.time + SOManager.Instance.Constants.RespawnTime;

        while (targetTime - Time.time > 0)
        {
            if (photonView.IsMine && !IsBot)
            {
                GameManager.Instance.ui.SetSpawnDelay(targetTime - Time.time);
            }

            yield return null;
        }

        GameManager.Instance.ui.DisableDeath();

        isRespawning = false;

        ResetAndRespawn();

        photonView.RPC("RpcSetAvailability", RpcTarget.All, true);
    }

    private void ResetAndRespawn()
    {
        ResetPosition(true);
    }

    #endregion
}