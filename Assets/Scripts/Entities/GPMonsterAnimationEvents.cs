using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TanksMP;

/// <summary>
/// This component has to be in the same gameobject that has teh animator so it can find
/// this animation events.
/// </summary>
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
