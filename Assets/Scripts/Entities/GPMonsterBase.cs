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

    public virtual void DamageMonster(Bullet bullet)
    {
        m_health.Damage(bullet.damage);

        Player other = bullet.owner.GetComponent<Player>();
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
        Vector3 targetDir = m_currTargetPlayer.transform.position - m_bulletSpawnPoint.position;
        Shoot(targetDir.normalized);
    }

    //shoots a bullet in the direction passed in
    //we do not rely on the current turret rotation here, because we send the direction
    //along with the shot request to the server to absolutely ensure a synced shot position
    protected void Shoot(Vector3 direction = default(Vector3))
    {
        //if shot delay is over  
        if (Time.time > m_nextFire)
        {
            //set next shot timestamp
            m_nextFire = Time.time + m_attackSpeed / 100f;

            //send current client position and turret rotation along to sync the shot position
            //also we are sending it as a short array (only x,z - skip y) to save additional bandwidth
            float[] pos = new float[] { m_bulletSpawnPoint.position.x, m_bulletSpawnPoint.position.z };
            //send shot request with origin to server
            this.photonView.RPC("CmdShoot", RpcTarget.AllViaServer, pos, direction);
        }
    }


    //called on the server first but forwarded to all clients
    [PunRPC]
    public void CmdShoot(float[] position, Vector3 forward)
    {
        //calculate center between shot position sent and current server position (factor 0.6f = 40% client, 60% server)
        //this is done to compensate network lag and smoothing it out between both client/server positions
        Vector3 shotCenter = Vector3.Lerp(m_bulletSpawnPoint.position, new Vector3(position[0], m_bulletSpawnPoint.position.y, position[1]), 0.6f);
        Quaternion syncedRot = Quaternion.LookRotation(forward);

        //spawn bullet using pooling
        GameObject obj = PoolManager.Spawn(m_bullet, shotCenter, syncedRot);
        obj.GetComponent<Bullet>().owner = gameObject;
        obj.GetComponent<Bullet>().ChangeDirection(forward);

        var trails = obj.GetComponentsInChildren<TrailRenderer>();

        foreach (var trail in trails)
        {
            trail.Clear();
        }

        //send event to all clients for spawning effects
        if (m_shotFX || m_shotClip)
            RpcOnShot();
    }


    //called on all clients after bullet spawn
    //spawn effects or sounds locally, if set
    protected void RpcOnShot()
    {
        if (m_shotFX) PoolManager.Spawn(m_shotFX, m_bulletSpawnPoint.position, Quaternion.identity);
        if (m_shotClip) AudioManager.Play3D(m_shotClip, m_bulletSpawnPoint.position, 0.1f);
    }

    public void OnPlayerKilled(Player playerKilled)
    {
        m_photonView.RPC("RPCOnPlayerKilled", RpcTarget.All, playerKilled.photonView.ViewID);
    }

    [PunRPC]
    public void RPCOnPlayerKilled(int playerId)
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
    }
}
