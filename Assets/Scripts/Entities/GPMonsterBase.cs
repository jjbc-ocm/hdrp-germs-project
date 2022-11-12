using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TanksMP;
using Photon.Pun;

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
    public GPGTriggerEvent m_meleeDamageTrigger;
    public GPGTriggerEvent m_goldTrigger;
    public GameObject m_model;

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
    [HideInInspector]
    public List<ActorManager> m_playersInGoldRange = new List<ActorManager>();
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

    Collider m_mainCollider;

    public void BaseStart()
    {
        if (m_photonView == null)
        {
            m_photonView = GetComponent<PhotonView>();
        }

        m_mainCollider = GetComponent<Collider>();

        if (PhotonNetwork.IsMasterClient)
        {
            m_health.OnDieEvent.AddListener(OnDie);
            m_health.OnDamagedEvent.AddListener(OnDamage);
            //m_detectionTrigger.m_OnEnterEvent.AddListener(OnPlayerEnter);
            //m_detectionTrigger.m_OnExitEvent.AddListener(OnPlayerExit);
            m_meleeDamageTrigger.m_OnEnterEvent.AddListener(BitePlayer);
            m_goldTrigger.m_OnEnterEvent.AddListener(OnPlayerGoldTriggerEnter);
            m_goldTrigger.m_OnExitEvent.AddListener(OnPlayerGoldTriggerExit);
        }

        m_respawnWorldPosition = transform.position;
        m_respawnModelLocalPosition = m_model.transform.localPosition;
    }

    public void LiveUpdate()
    {
        
    }

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

    public virtual void ChoosePlayerToAttack()
    {
        if (m_playersInRange.Count == 0)
        {
            return;
        }
        int playerIdx = Random.Range(0, m_playersInRange.Count);
        m_currTargetPlayer = m_playersInRange[playerIdx];
    }

    public virtual void OnDamage()
    {
        if (!m_attacking)
        {
            m_photonView.RPC("RPCPlayAnimationTrigger", RpcTarget.All, m_hurtTriggerName);
        }
        m_photonView.RPC("RPCOnHurt", RpcTarget.All);
    }

    public virtual void OnDie()
    {
        m_photonView.RPC("RPCPlayAnimationTrigger", RpcTarget.All, m_dieTriggerName);
        m_photonView.RPC("RPCOnDie", RpcTarget.All);
        GiveRewards();
        StartCoroutine(Sink());
    }

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

    public IEnumerator Emerge()
    {
        transform.position = m_respawnWorldPosition;
        m_model.transform.localPosition = m_respawnModelLocalPosition - (Vector3.up * 10.0f);
        LeanTween.moveLocalY(m_model, m_respawnModelLocalPosition.y, m_emergeTime).setEaseSpring();
        yield return 0;
    }

    [PunRPC]
    public override void RpcDamageHealth(int amount, int attackerId)
    {
        Debug.Log("RpcDamageHealth");

        m_health.Damage(amount);

        var other = PhotonView.Find(attackerId)?.GetComponent<ActorManager>() ?? null;//bullet.Owner;

        if (other)
        {
            m_lastHitPlayer = other;
            if (!m_playersWhoDamageIt.Contains(other))
            {
                m_playersWhoDamageIt.Add(other);
            }
        }
    }

    /*public virtual void DamageMonster(BulletManager bullet)
    {
        m_health.Damage(bullet.Damage);

        var other = bullet.Owner;

        if (other)
        {
            m_lastHitPlayer = other;
            if (!m_playersWhoDamageIt.Contains(other))
            {
                m_playersWhoDamageIt.Add(other);
            }
        }

    }*/

    public virtual void DamagePlayer(ActorManager player)
    {
        MonsterMeleeAttackDesc meleeAtk = (MonsterMeleeAttackDesc)m_currAtk;
        if (meleeAtk == null) { return; }
        //player.TakeMonsterDamage(meleeAtk);
        player.photonView.RPC("RpcDamageHealth", RpcTarget.All, meleeAtk, photonView.ViewID);
    }

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

    public virtual void OnPlayerGoldTriggerEnter(Collider other)
    {
        ActorManager player = other.GetComponent<ActorManager>();
        if (player)
        {
            if (!m_playersInGoldRange.Contains(player))
            {
                m_playersInGoldRange.Add(player);
            }
        }
    }

    public virtual void OnPlayerGoldTriggerExit(Collider other)
    {
        ActorManager player = other.GetComponent<ActorManager>();
        if (player)
        {
            if (m_playersInGoldRange.Contains(player))
            {
                m_playersInGoldRange.Remove(player);
            }
        }
    }

    public void GiveRewards()
    {
        //get winning team
        int team = m_lastHitPlayer.photonView.GetTeam();
        foreach (ActorManager player in m_playersWhoDamageIt)
        {
            if (player.photonView.GetTeam() == team && m_playersInGoldRange.Contains(player))
            {
                GPRewardSystem.m_instance.AddGoldToPlayer(player.photonView.Owner, m_rewardKey);
            }
        }
    }

    //For melee attacks, if I rename teh method animation events will be lost
    public void StartMeleeAttack()
    {
        MonsterMeleeAttackDesc meleeAtk = (MonsterMeleeAttackDesc)m_currAtk;
        if (m_currTargetPlayer == null || meleeAtk == null)
        {
            EndMeleeAttack();
            return;
        }

        if (meleeAtk.m_damageType == DamageDetectionType.kAlwaysDamageTarget)
        {
            //m_currTargetPlayer.TakeMonsterDamage(meleeAtk);
            m_currTargetPlayer.photonView.RPC("RpcDamageHealth", RpcTarget.All, meleeAtk, photonView.ViewID);
        }
        else if (meleeAtk.m_damageType == DamageDetectionType.kDamageOnCollision)
        {
            m_meleeDamageTrigger.SetEnabled(true);
        }
    }

    public void EndMeleeAttack()
    {
        m_meleeDamageTrigger.SetEnabled(false);
    }

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

    public virtual void BitePlayer(Collider collider)
    {
        Player player = collider.GetComponent<Player>();
        MonsterMeleeAttackDesc meleeAtk = m_currAtk as MonsterMeleeAttackDesc;
        if (meleeAtk == null) { return; }

        if (player && player == m_currTargetPlayer)
        {
            m_photonView.RPC("RPCOnMeleeHit", RpcTarget.All);
            player.TakeMonsterDamage(meleeAtk);
        }

        EndMeleeAttack();
    }

    //Shooting

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
        var canExecute = Time.time > m_nextFire && photonView.GetMana() >= action.MpCost;

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

        photonView.SetMana(photonView.GetMana() - action.MpCost);

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

        effect.GetComponent<SkillBaseManager>().Initialize(this); // TODO: BulletManager = this is not always the case // TODO: 3 and 4
    }

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

        if (m_playersInGoldRange.Contains(playerKilled))
        {
            m_playersInGoldRange.Remove(playerKilled);
        }

        playerKilled.onDieEvent.RemoveListener(OnPlayerKilled);
    }

    [PunRPC]
    public void RPCPlayAnimationTrigger(string triggerName)
    {
        m_animator.SetTrigger(triggerName);
    }

    [PunRPC]
    public void RPCOnHurt()
    {
        AudioManager.Play3D(m_hurtSFX, transform.position, 0.1f);
    }

    [PunRPC]
    public void RPCOnDie()
    {
        m_mainCollider.enabled = false;
        AudioManager.Play3D(m_deathSFX, transform.position, 0.1f);
    }

    [PunRPC]
    public void RPCOnRespawn()
    {
        m_mainCollider.enabled = true;
        m_respawnTimeCounter = 0.0f;
        m_health.Resurrect();
        m_animator.Play("Idle");
        AudioManager.Play3D(m_deathSFX, transform.position, 0.1f);
    }

    [PunRPC]
    public void RPCOnMeleeHit()
    {
        AudioManager.Play3D(m_meleeAtkHitSFX, transform.position, 0.1f);
    }
}
