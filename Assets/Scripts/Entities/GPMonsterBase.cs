using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TanksMP;
using Photon.Pun;

public class GPMonsterBase : MonoBehaviourPunCallbacks
{
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

    [Header("Component references")]
    public PhotonView m_photonView;
    public GPHealth m_health;
    public GPGTriggerEvent m_detectionTrigger;
    public GPGTriggerEvent m_meleeDamageTrigger;

    [Header("Movement settings")]
    public float m_rotateSpeed = 3.0f;
    [HideInInspector]
    public bool m_tweening = false;

    [Header("Animation settings")]
    public Animator m_animator;
    public List<string> m_meleeAtkTriggerNames;
    public List<string> m_projectileAtkTriggerNames;
    public string m_dieTriggerName = "Die";
    public string m_hurtTriggerName = "Take damage";

    [Header("Attack settings")]
    public AttackPickType m_pickType;
    public DamageDetectionType m_damageDetectionType;
    [SerializeField]
    private SkillData attack;
    [SerializeField]
    private SkillData skill;
    int m_currAttkIdx = 0;
    public int m_damagePoints = 100;
    public float m_meleeRadius = 3.0f;
    public float m_minAttackTime = 1.0f;
    public float m_maxAttackTime = 3.5f;
    [HideInInspector]
    public float m_nextAttackTime = 0.0f;
    [HideInInspector]
    public float m_nextAttackTimeCounter = 0.0f;
    [HideInInspector]
    public Player m_currTargetPlayer;

    [HideInInspector]
    public List<Player> m_playersInRange = new List<Player>();
    [HideInInspector]
    public List<Player> m_playersWhoDamageIt = new List<Player>();
    [HideInInspector]
    public Player m_lastHitPlayer;

    [Header("Death settings")]
    public float m_destroyTime = 1.0f;
    public float m_sinkSpeed = 2.0f;

    [Header("Gold settings")]
    [Tooltip("This key should be defined on the reward system prefab.")]
    public string m_rewardKey = "defaultMonster";

    [Header("Shot settings")]
    public Transform m_bulletSpawnPoint;
    [SerializeField]
    private GameObject m_shotFX;
    [SerializeField]
    private AudioClip m_shotClip;
    private float m_nextFire;
    [SerializeField]
    private GameObject m_bullet;
    [SerializeField]
    private int m_attackSpeed = 50;
    private bool isExecutingActionAim;
    private bool isExecutingActionAttack;

    private void Start()
    {
        if (m_photonView == null)
        {
            m_photonView = GetComponent<PhotonView>();
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
        m_animator.SetTrigger(m_hurtTriggerName);
    }

    public virtual void OnDie()
    {
        m_animator.SetTrigger(m_dieTriggerName);
        GiveRewards();
        StartCoroutine(Sink());
    }

    public IEnumerator Sink()
    {
        float timeCounter = 0.0f;
        while (timeCounter <= m_destroyTime)
        {
            timeCounter += Time.fixedDeltaTime;
            transform.position -= Vector3.up * Time.fixedDeltaTime * m_sinkSpeed;
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
    }

    public virtual void DamageMonster(BulletManager bullet)
    {
        m_health.Damage(bullet.Damage);

        Player other = bullet.Owner;
        if (other)
        {
            m_lastHitPlayer = other;
            if (!m_playersWhoDamageIt.Contains(other))
            {
                m_playersWhoDamageIt.Add(other);
            }
        }

    }

    public virtual void DamagePlayer(Player player)
    {
        player.TakeMonsterDamage(this);
    }

    public virtual void OnPlayerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
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
        Player player = other.GetComponent<Player>();
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

    public void GiveRewards()
    {
        //get winning team
        int team = m_lastHitPlayer.photonView.GetTeam();
        foreach (Player player in m_playersWhoDamageIt)
        {
            if (player.photonView.GetTeam() == team && m_playersInRange.Contains(player))
            {
                GPRewardSystem.m_instance.AddGoldToPlayer(player.photonView.Owner, m_rewardKey);
            }
        }
    }

    public void StartAttack()
    {
        if (m_damageDetectionType == DamageDetectionType.kAlwaysDamageTarget)
        {
            m_currTargetPlayer.TakeMonsterDamage(this);
        }
        else if (m_damageDetectionType == DamageDetectionType.kDamageOnCollision)
        {
            m_meleeDamageTrigger.SetEnabled(true);
        }
    }

    public void EndAttack()
    {
        m_meleeDamageTrigger.SetEnabled(false);
    }

    public string PickAttack(List<string> atkList)
    {
        int attackIndex = 0;
        if (m_pickType == AttackPickType.kRandom)
        {
            attackIndex = Random.Range(0, atkList.Count);
        }
        else if (m_pickType == AttackPickType.kSecuence)
        {
            attackIndex = m_currAttkIdx;
            m_currAttkIdx++;
            if (m_currAttkIdx >= atkList.Count) // loop the attacks
            {
                m_currAttkIdx = 0;
            }
        }
        return atkList[attackIndex];

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
    }

    //Shooting

    public void ShootAnimEvent()
    {
        if (m_currTargetPlayer == null)
        {
            return;
        }
        ExecuteAction(attack, true);
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
        var action = isAttack ? attack : skill;

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
        var action = isAttack ? attack : skill;

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

        effect.GetComponent<BulletManager>().Initialize(this); // TODO: BulletManager = this is not always the case // TODO: 3 and 4
    }

    public void OnPlayerKilled(int playerId)
    {
        PhotonView playerView = playerId > 0 ? PhotonView.Find(playerId) : null;
        Player playerKilled = playerView.GetComponent<Player>();

        if (m_currTargetPlayer == playerKilled)
        {
            m_currTargetPlayer = null;
        }

        if (m_playersInRange.Contains(playerKilled))
        {
            m_playersInRange.Remove(playerKilled);
        }

        playerKilled.onDieEvent.RemoveListener(OnPlayerKilled);
    }
}
