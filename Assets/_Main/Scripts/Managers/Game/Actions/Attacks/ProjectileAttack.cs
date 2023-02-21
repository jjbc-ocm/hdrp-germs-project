
using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using TanksMP;

public class ProjectileAttack : AttackBase
{
    [SerializeField]
    private AudioClip hitClip;

    [SerializeField]
    private AudioClip explosionClip;

    [SerializeField]
    private GameObject hitFX;

    [SerializeField]
    private GameObject explosionFX;

    private Rigidbody rigidBody;


    #region Unity

    void Start()
    {
        Destroy(gameObject, Constants.FOG_OF_WAR_DISTANCE / velocity);
    }

    void OnTriggerEnter(Collider col)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (col.CompareTag("IgnoreBullet")) // ignore collision
        {
            return;
        }

        if (col.TryGetComponent(out ActorManager actor))
        {
            if (!IsHit(owner, actor)) return;

            ApplyEffect(owner, actor);

            if (hitFX) PoolManager.Spawn(hitFX, transform.position, Quaternion.identity);

            if (hitClip) AudioManager.Instance.Play3D(hitClip, transform.position);

            Destroy(gameObject);
        }
    }

    #endregion

    protected override void OnInitialize()
    {
        rigidBody = GetComponent<Rigidbody>();

        rigidBody.velocity = transform.forward * velocity;
    }
}
