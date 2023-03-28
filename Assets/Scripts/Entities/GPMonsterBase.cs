using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TanksMP;
using Photon.Pun;
using System.Linq;

public enum AttackPickType
{
    kSecuence, // iterate teh attacks in order
    kRandom, // pick a random attack from the list
}

public enum DamageDetectionType
{
    kAlwaysDamageTarget, // always damage targeted player
    kDamageOnCollision, // damage only if the targeted player was inside the melee trigger
}

public enum MONSTER_STATES
{
    kIdle,
    kAttacking,
}

[System.Serializable]
public class MonsterAttackDesc
{
    public string m_triggerName;
    public float m_duration = 1.0f;
}

[System.Serializable]
public class MonsterMeleeAttackDesc : MonsterAttackDesc
{
  public DamageDetectionType m_damageType;
  public int m_damage = 10;
}

[System.Serializable]
public class MonsterProjectileAttackDesc : MonsterAttackDesc
{
  [SerializeField]
  public SkillData attack;
}

public class GPMonsterBase : ActorManager
{
    [Header("Component references")]
    public PhotonView m_photonView;
    public GPHealth m_health;
    public GPGTriggerEvent m_detectionTrigger;
    [Tooltip("When player enters this trigger he will be damage, this trigger is actiavted using animation events")]
    public GPGTriggerEvent m_meleeDamageTrigger;
    //[Tooltip("Only the players that are inside it when the mosnter dies will get gold (last hit team only)")]
    //public GPGTriggerEvent m_goldTrigger;
    public GameObject m_model; //Monster model gameobject
    public GameObject m_monsterHPBar;

    [Header("Movement settings")]
    public float m_rotateSpeed = 3.0f;
    [HideInInspector]
    public bool m_tweening = false;

    [Header("Animation settings")]
    public Animator m_animator;
    public List<MonsterMeleeAttackDesc> m_meleeAtks;
    public List<MonsterProjectileAttackDesc> m_projectileAtks;
    public string m_dieTriggerName = "Die";
    public string m_hurtTriggerName = "Take damage";

    [Header("Attack settings")]
    public AttackPickType m_pickType;
    [SerializeField]
    private SkillData skill;
    int m_currMeleeAttkIdx = 0;
    int m_currProjectileAttkIdx = 0;
    public float m_meleeRadius = 3.0f;
    public float m_projectileRadius = 100.0f;
    public float m_minAttackTime = 3.5f;
    public float m_maxAttackTime = 4.0f;
    [HideInInspector]
    public float m_nextAttackTime = 0.0f;
    [HideInInspector]
    public float m_nextAttackTimeCounter = 0.0f;
    [HideInInspector]
    public ActorManager m_currTargetPlayer;
    public MonsterAttackDesc m_currAtk;
    [HideInInspector]
    public bool m_attacking = false;

    [HideInInspector]
    public List<ActorManager> m_playersInRange = new List<ActorManager>();
    //[HideInInspector]
    //public List<ActorManager> m_playersInGoldRange = new List<ActorManager>();
    [HideInInspector]
    public List<ActorManager> m_playersWhoDamageIt = new List<ActorManager>();
    [HideInInspector]
    public ActorManager m_lastHitPlayer;

    [Header("Death settings")]
    public float m_destroyTime = 1.0f;
    public float m_sinkSpeed = 2.0f;

    [Header("Respawn settings")]
    [Tooltip("Time until monster is respawned. In Seconds.")]
    public float m_respawnTime = 120.0f;
    float m_respawnTimeCounter = 0.0f;
    [HideInInspector]
    public Vector3 m_respawnWorldPosition;
    public Vector3 m_respawnModelLocalPosition;
    public float m_emergeTime = 1.0f;

    [Header("Gold settings")]
    [Tooltip("This key should be defined on the reward system prefab.")]
    public string m_rewardKey = "defaultMonster";

    [Header("Shot settings")]
    public Transform m_bulletSpawnPoint;
    [SerializeField]
    private float m_nextFire;
    [SerializeField]
    private int m_attackSpeed = 50;
    private bool isExecutingActionAim;
    private bool isExecutingActionAttack;

    [Header("Sound settings")]
    public AudioClip m_hurtSFX;
    public AudioClip m_deathSFX;
    public AudioClip m_meleeAtkHitSFX;

    [Header("Visuals settings")]
    public GameObject[] m_renderers;

    Collider m_mainCollider;

    public void BaseStart()
    {
        //Get photon view
        if (m_photonView == null)
        {
            m_photonView = GetComponent<PhotonView>();
        }

        m_mainCollider = GetComponent<Collider>();

        //Only master client will process events
        if (PhotonNetwork.IsMasterClient)
        {
            m_health.OnDieEvent.AddListener(OnDie);
            m_health.OnDamagedEvent.AddListener(OnDamage);
            //m_detectionTrigger.m_OnEnterEvent.AddListener(OnPlayerEnter);
            //m_detectionTrigger.m_OnExitEvent.AddListener(OnPlayerExit);
            m_meleeDamageTrigger.m_OnEnterEvent.AddListener(BitePlayer);
            //m_goldTrigger.m_OnEnterEvent.AddListener(OnPlayerGoldTriggerEnter);
            //m_goldTrigger.m_OnExitEvent.AddListener(OnPlayerGoldTriggerExit);
        }

        //store respawn position.
        m_respawnWorldPosition = transform.position;
        m_respawnModelLocalPosition = m_model.transform.localPosition;
    }

    /// <summary>
    /// Logic that updates while the monster is alive.
    /// </summary>
    public void LiveUpdate()
    {
        
    }

    /// <summary>
    /// Logic that updates while the monster is dead.
    /// </summary>
    public void DeathUpdate()
    {
        if (m_health.m_isDead)
        {
            m_respawnTimeCounter += Time.deltaTime;
            if (m_respawnTimeCounter >= m_respawnTime)
            {
                StartCoroutine(Emerge());
                m_photonView.RPC("RPCOnRespawn", RpcTarget.All);
            }
        }
    }

    /// <summary>
    /// Randomly chooses a player in range to attack.
    /// </summary>
    public virtual void ChoosePlayerToAttack()
    {
        if (m_playersInRange.Count == 0)
        {
            return;
        }
        int playerIdx = Random.Range(0, m_playersInRange.Count);
        m_currTargetPlayer = m_playersInRange[playerIdx];
    }

    /// <summary>
    /// Logic to do when monster is damaged.
    /// </summary>
    public virtual void OnDamage()
    {
        if (!m_attacking)
        {
            m_photonView.RPC("RPCPlayAnimationTrigger", RpcTarget.All, m_hurtTriggerName);
        }
        m_photonView.RPC("RPCOnHurt", RpcTarget.All);
    }

    /// <summary>
    /// Logic to do when monster dies.
    /// </summary>
    public virtual void OnDie()
    {
        m_photonView.RPC("RPCPlayAnimationTrigger", RpcTarget.All, m_dieTriggerName);
        m_photonView.RPC("RPCOnDie", RpcTarget.All);
        GiveRewards();
        StartCoroutine(Sink());
    }

    /// <summary>
    /// Play sink animation
    /// </summary>
    /// <returns></returns>
    public IEnumerator Sink()
    {
        float timeCounter = 0.0f;
        while (timeCounter <= m_destroyTime)
        {
            timeCounter += Time.fixedDeltaTime;
            m_model.transform.localPosition -= Vector3.up * Time.fixedDeltaTime * m_sinkSpeed;
            yield return new WaitForFixedUpdate();
        }
        //Destroy(gameObject);
    }

    /// <summary>
    /// Plays emerge animation
    /// </summary>
    /// <returns></returns>
    public IEnumerator Emerge()
    {
        transform.position = m_respawnWorldPosition;
        m_model.transform.localPosition = m_respawnModelLocalPosition - (Vector3.up * 10.0f);
        LeanTween.moveLocalY(m_model, m_respawnModelLocalPosition.y, m_emergeTime).setEaseSpring();
        yield return 0;
    }

    /// <summary>
    /// Decreases monster's health.
    /// Stores the players who damaged the monster in a list.
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="attackerId"></param>
    [PunRPC]
    public override void RpcDamageHealth(int amount, int attackerId)
    {
        var attacker = PhotonView.Find(attackerId)?.GetComponent<ActorManager>() ?? null;

        if (attacker)
        {
            m_lastHitPlayer = attacker;

            if (!m_playersWhoDamageIt.Contains(attacker))
            {
                m_playersWhoDamageIt.Add(attacker);
            }
        }

        if (attacker == PlayerManager.Mine && !IsBot)
        {
            PopupManager.Instance.ShowDamage(amount, transform.position);
        }
        
        m_health.Damage(amount);
    }

    /// <summary>
    /// Does damage to a player
    /// </summary>
    /// <param name="player"></param>
    public virtual void DamagePlayer(ActorManager player)
    {
        MonsterMeleeAttackDesc meleeAtk = m_currAtk as MonsterMeleeAttackDesc;
        if (meleeAtk == null) { return; }
        //player.TakeMonsterDamage(meleeAtk);
        player.photonView.RPC("RpcDamageHealth", RpcTarget.All, meleeAtk.m_damage, photonView.ViewID);
    }

    /// <summary>
    /// Registers a player inside monster's range.
    /// </summary>
    /// <param name="other"></param>
    public virtual void OnPlayerEnter(Collider other)
    {
        ActorManager player = other.GetComponent<ActorManager>();
        if (player)
        {
            if (!m_playersInRange.Contains(player))
            {
                m_playersInRange.Add(player);
                player.onDieEvent.AddListener(OnPlayerKilled);
            }
            m_currTargetPlayer = player;
        }
    }

    /// <summary>
    /// Erase a player from the monster's range list.
    /// </summary>
    /// <param name="other"></param>
    public virtual void OnPlayerExit(Collider other)
    {
        ActorManager player = other.GetComponent<ActorManager>();
        if (player)
        {
            if (m_playersInRange.Contains(player))
            {
                m_playersInRange.Remove(player);
            }
        }

        if (player == m_currTargetPlayer)
        {
            m_currTargetPlayer = null;
        }
    }

    /// <summary>
    /// Registers a player inside monster's gold range.
    /// </summary>
    /// <param name="other"></param>
    /// // TODO: unused method remove someday
    /*public virtual void OnPlayerGoldTriggerEnter(Collider other)
    {
        ActorManager player = other.GetComponent<ActorManager>();
        if (player)
        {
            if (!m_playersInGoldRange.Contains(player))
            {
                m_playersInGoldRange.Add(player);
            }
        }
    }*/

    /// <summary>
    /// Erase a player from the monster's gold range list.
    /// </summary>
    /// <param name="other"></param>
    /// // TODO: unused method remove someday
    /*public virtual void OnPlayerGoldTriggerExit(Collider other)
    {
        ActorManager player = other.GetComponent<ActorManager>();
        if (player)
        {
            if (m_playersInGoldRange.Contains(player))
            {
                m_playersInGoldRange.Remove(player);
            }
        }
    }*/

    /// <summary>
    /// Gives gold to all players in gold range that are from the team that made the last hit.
    /// </summary>
    public void GiveRewards()
    {
        //get winning team
        var team = m_lastHitPlayer.GetTeam();

        // filter the players who must receive the reward by team and range
        var rewardedPlayers = m_playersWhoDamageIt.Where(i => i.GetTeam() == team && IsVisibleRelativeTo(i.transform));

        /*foreach (ActorManager player in m_playersWhoDamageIt)
        {
            if (player.GetTeam() == team && m_playersInGoldRange.Contains(player))
            {
                GPRewardSystem.m_instance.AddGoldToPlayer(player, m_rewardKey);
                m_photonView.RPC("RPCSpawnCoinsForPlayer", player.photonView.Owner);

                if (player.photonView.IsMine && !player.IsBot)
                {
                    PopupManager.Instance.ShowGold(GPRewardSystem.m_instance.GetRewardAmountByKey(m_rewardKey), transform.position);
                }
            }
        }*/

        foreach (var rewardedPlayer in rewardedPlayers)
        {
            var amount = Mathf.CeilToInt(GPRewardSystem.m_instance.m_rewardsMap[m_rewardKey] / (float)rewardedPlayers.Count());

            rewardedPlayer.photonView.RPC("AddGold", RpcTarget.All, amount);

            GPRewardSystem.m_instance.SpawnCoins(transform.position, amount, rewardedPlayer.transform);

            if (rewardedPlayer.photonView.IsMine && !rewardedPlayer.IsBot)
            {
                PopupManager.Instance.ShowGold(amount, rewardedPlayer.transform.position);
            }
        }
    }

    /// <summary>
    /// AnimationEvent to start a melee attack.
    /// Activates damage triggers.
    /// Do not rename or the aniamtione vent will be lost.
    /// </summary>
    public void StartMeleeAttack()
    {
        MonsterMeleeAttackDesc meleeAtk = m_currAtk as MonsterMeleeAttackDesc;
        if (m_currTargetPlayer == null || meleeAtk == null)
        {
            EndMeleeAttack();
            return;
        }

        if (meleeAtk.m_damageType == DamageDetectionType.kAlwaysDamageTarget)
        {
            m_currTargetPlayer.photonView.RPC("RpcDamageHealth", RpcTarget.All, meleeAtk.m_damage, photonView.ViewID);
        }
        else if (meleeAtk.m_damageType == DamageDetectionType.kDamageOnCollision)
        {
            m_meleeDamageTrigger.SetEnabled(true);
        }
    }

    /// <summary>
    /// Disables melee damage trigger.
    /// </summary>
    public void EndMeleeAttack()
    {
        m_meleeDamageTrigger.SetEnabled(false);
    }

    /// <summary>
    /// Chooses a melee attack from the list.
    /// </summary>
    /// <returns></returns>
    public MonsterAttackDesc PickMeleeAttack()
    {
        int attackIndex = 0;
        if (m_pickType == AttackPickType.kRandom)
        {
            attackIndex = Random.Range(0, m_meleeAtks.Count);
        }
        else if (m_pickType == AttackPickType.kSecuence)
        {
            attackIndex = m_currMeleeAttkIdx;
            m_currMeleeAttkIdx++;
            if (m_currMeleeAttkIdx >= m_meleeAtks.Count) // loop the attacks
            {
                m_currMeleeAttkIdx = 0;
            }
        }

        m_currAtk = m_meleeAtks[attackIndex];

        return m_meleeAtks[attackIndex];
    }

    /// <summary>
    /// Chooses a projectile attack from the list.
    /// </summary>
    /// <returns></returns>
    public MonsterAttackDesc PickProjectileAttack()
    {
        int attackIndex = 0;
        if (m_pickType == AttackPickType.kRandom)
        {
            attackIndex = Random.Range(0, m_projectileAtks.Count);
        }
        else if (m_pickType == AttackPickType.kSecuence)
        {
            attackIndex = m_currProjectileAttkIdx;
            m_currProjectileAttkIdx++;
            if (m_currProjectileAttkIdx >= m_projectileAtks.Count) // loop the attacks
            {
                m_currProjectileAttkIdx = 0;
            }
        }

        m_currAtk = m_projectileAtks[attackIndex];

        return m_projectileAtks[attackIndex];
    }

    /// <summary>
    /// Rotates mosnter to llok in the direction of the target.
    /// Needs to be called on update.
    /// </summary>
    /// <param name="target"></param>
    public virtual void LookAtTarget(Transform target)
    {
        Vector3 lookDir = target.transform.position - transform.position;
        lookDir.y = 0.0f;
        transform.forward = Vector3.Lerp(transform.forward, lookDir, Time.deltaTime * m_rotateSpeed);
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, m_meleeRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_projectileRadius);
    }

    /// <summary>
    /// Does melee damage to a player.
    /// </summary>
    /// <param name="collider"></param>
    public virtual void BitePlayer(Collider collider)
    {
        var player = collider.GetComponent<PlayerManager>();
        MonsterMeleeAttackDesc meleeAtk = m_currAtk as MonsterMeleeAttackDesc;
        if (meleeAtk == null) { return; }

        if (player && player == m_currTargetPlayer)
        {
            m_photonView.RPC("RPCOnMeleeHit", RpcTarget.All);
            //player.TakeMonsterDamage(meleeAtk);
            player.photonView.RPC("RpcDamageHealth", RpcTarget.All, meleeAtk.m_damage, photonView.ViewID);
        }

        EndMeleeAttack();
    }

    /// <summary>
    /// Shoots a projectile
    /// </summary>
    public void ShootProjectile()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        MonsterProjectileAttackDesc projAtk = m_currAtk as MonsterProjectileAttackDesc;
        if (m_currTargetPlayer == null || projAtk == null)
        {
            return;
        }

        ExecuteAction(projAtk.attack, true);
    }

    private void ExecuteAction(SkillData action, bool isAttack)
    {
        var canExecute = Time.time > m_nextFire;// && photonView.GetMana() >= action.MpCost;

        if (canExecute)
        {
            m_nextFire = Time.time + m_attackSpeed / 100f;

            var instantAim =
                action.Aim == AimType.None ? m_currTargetPlayer.transform.position :
                action.Aim == AimType.WhileExecute ? m_currTargetPlayer.transform.position :
                Vector3.zero;

            if (action.Aim == AimType.None || action.Aim == AimType.WhileExecute)
            {
                ExecuteActionInstantly(instantAim, isAttack);
            }
            else
            {
                isExecutingActionAim = true;

                isExecutingActionAttack = isAttack;
            }
        }
    }

    private void ExecuteActionInstantly(Vector3 aimPosition, bool isAttack)
    {
        MonsterProjectileAttackDesc projAtk = (MonsterProjectileAttackDesc)m_currAtk;
        if (projAtk == null) { return; }
        var action = isAttack ? projAtk.attack : skill;

        isExecutingActionAim = false;

        //photonView.SetMana(photonView.GetMana() - action.MpCost);

        photonView.RPC(
            "RpcAction",
            RpcTarget.AllViaServer,
            new float[] { m_bulletSpawnPoint.position.x, m_bulletSpawnPoint.position.y, m_bulletSpawnPoint.position.z },
            new float[] { aimPosition.x, aimPosition.y, aimPosition.z },
            isAttack);
    }

    [PunRPC]
    public void RpcAction(float[] position, float[] target, bool isAttack)
    {
        MonsterProjectileAttackDesc projAtk = (MonsterProjectileAttackDesc)m_currAtk;
        var action = isAttack ? projAtk.attack : skill;

        /* Steps
         * 1. Calculate the rotation ased on position and target
         * 2. Spawn action.Effect based on position, and rotation
         * 3. pass the action (SkillData) as parameter to the spawned object
         * 4. Any trail reset or sound effects should be done on the actual object spawned
         */
        var vPosition = new Vector3(position[0], position[1], position[2]);

        var vTarget = new Vector3(target[0], target[1], target[2]);

        var forward = vTarget - vPosition;

        var rotation = Quaternion.LookRotation(forward);

        var effect = Instantiate(action.Effect, vPosition, rotation);

        effect.GetComponent<ActionBase>().Initialize(this, Vector3.zero); // TODO: ProjectileAttack = this is not always the case // TODO: 3 and 4
    }

    /// <summary>
    /// Logic to do when the monster kills a player.
    /// Player is removed from gold range and attack range lists.
    /// </summary>
    /// <param name="playerId"></param>
    public void OnPlayerKilled(int playerId)
    {
        PhotonView playerView = playerId > 0 ? PhotonView.Find(playerId) : null;
        ActorManager playerKilled = playerView.GetComponent<ActorManager>();

        if (m_currTargetPlayer == playerKilled)
        {
            m_currTargetPlayer = null;
        }

        if (m_playersInRange.Contains(playerKilled))
        {
            m_playersInRange.Remove(playerKilled);
        }

        /*if (m_playersInGoldRange.Contains(playerKilled))
        {
            m_playersInGoldRange.Remove(playerKilled);
        }*/

        playerKilled.onDieEvent.RemoveListener(OnPlayerKilled);
    }

    [PunRPC]
    public void RPCPlayAnimationTrigger(string triggerName)
    {
        m_animator.SetTrigger(triggerName);
    }

    public UnityEvent OnRPCHurtEvent;
    [PunRPC]
    public void RPCOnHurt()
    {
        AudioManager.Instance.Play3D(m_hurtSFX, transform.position);
        if (OnRPCHurtEvent != null)
        {
            OnRPCHurtEvent.Invoke();
        }
    }

    [PunRPC]
    public void RPCOnDie()
    {
        m_mainCollider.enabled = false;
        AudioManager.Instance.Play3D(m_deathSFX, transform.position);
        if (m_monsterHPBar)
        {
            m_monsterHPBar.SetActive(false);
        }
    }

    /// <summary>
    /// Spawns visual coins for the local player
    /// </summary>
    /*[PunRPC]
    public void RPCSpawnCoinsForPlayer()
    {
        //search player
        foreach (ActorManager player in m_playersWhoDamageIt)
        {
            if (player.photonView.Owner == PhotonNetwork.LocalPlayer)
            {
                GPRewardSystem.m_instance.SpawnCoins(transform.position, GPRewardSystem.m_instance.m_rewardsMap[m_rewardKey], player.transform);
            }
        }
    }*/

    /// <summary>
    /// Resets monsters health, colliders, animator, among other things.
    /// </summary>
    [PunRPC]
    public void RPCOnRespawn()
    {
        m_mainCollider.enabled = true;
        m_respawnTimeCounter = 0.0f;
        if (m_monsterHPBar)
        {
            m_monsterHPBar.SetActive(true);
        }
        m_health.Resurrect();
        m_animator.Play("Idle");
        AudioManager.Instance.Play3D(m_deathSFX, transform.position);
    }

    /// <summary>
    /// Visual and auditive effects that needs to happen for all players when the monster receives a hit.
    /// </summary>
    [PunRPC]
    public void RPCOnMeleeHit()
    {
        AudioManager.Instance.Play3D(m_meleeAtkHitSFX, transform.position);
    }

    protected override void OnTriggerEnterCalled(Collider col)
    {

    }

    protected override void OnTriggerExitCalled(Collider col)
    {

    }
}
