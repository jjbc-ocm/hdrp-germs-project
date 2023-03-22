using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TanksMP;

public class GPMonstersPack : MonoBehaviour
{
    public GPGTriggerEvent m_detectionTrigger;
    public List<GPMonsterBase> m_monsters;

    // Start is called before the first frame update
    void Start()
    {
        m_detectionTrigger.m_OnEnterEvent.AddListener(OnPlayerEnter);
        m_detectionTrigger.m_OnExitEvent.AddListener(OnPlayerExit);
    }

    /// <summary>
    /// Linked to the detection trigger enter event.
    /// When player enters all monsters will be notified.
    /// </summary>
    /// <param name="other"></param>
    public void OnPlayerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerManager>();
        if (player)
        {
            foreach (var monster in m_monsters)
            {
                monster.OnPlayerEnter(other);
            }
        }
    }

    // <summary>
    /// Linked to the detection trigger exit event.
    /// When player exits all monsters will be notified.
    /// </summary>
    /// <param name="other"></param>
    public void OnPlayerExit(Collider other)
    {
        var player = other.GetComponent<PlayerManager>();
        if (player)
        {
            foreach (var monster in m_monsters)
            {
                monster.OnPlayerExit(other);
            }
        }

        //If a bee exit his nest zone.
        GPBee bee = other.GetComponent<GPBee>();
        if (bee)
        {
            if (m_monsters.Contains(bee)) // if that bee is from this nest
            {
                bee.ReturnToNest();
            }
        }

    }
}
