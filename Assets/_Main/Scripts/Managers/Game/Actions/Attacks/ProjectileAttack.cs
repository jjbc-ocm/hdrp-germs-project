
using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using TanksMP;
using System.Collections;

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

    /*void Start()
    {
        Destroy(gameObject, SOManager.Instance.Constants.FogOrWarDistance / velocity);
    }*/

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

            if (hitFX) Instantiate(hitFX, transform.position, Quaternion.identity);

            if (hitClip) AudioManager.Instance.Play3D(hitClip, transform.position);

            Destroy(gameObject);
        }
    }

    #endregion

    #region Override

    protected override void OnInitialize()
    {
        rigidBody = GetComponent<Rigidbody>();

        rigidBody.velocity = transform.forward * velocity;
    }

    public override void OnGet()
    {
        StartCoroutine(YieldDespawn());
    }

    public override void OnRelease()
    {

    }

    #endregion

    #region Private

    private IEnumerator YieldDespawn()
    {
        yield return new WaitForSeconds(SOManager.Instance.Constants.FogOrWarDistance / velocity);

        PoolManager.Instance.Release(this);
    }

    #endregion
}
