using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TanksMP;

public class GPMonsterAnimationEvents : MonoBehaviour
{
    public GPMonsterBase m_monsterOwner;

    public void StartMeleeAttack()
    {
        m_monsterOwner.StartMeleeAttack();
    }

    public void EndMeleeAttack()
    {
        m_monsterOwner.EndMeleeAttack();
    }

    public void ShootAnimEvent()
    {
        m_monsterOwner.ShootProjectile();
    }
}
