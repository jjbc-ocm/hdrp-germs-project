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

    public void OnPlayerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player)
        {
            foreach (var monster in m_monsters)
            {
                monster.OnPlayerEnter(other);
            }
        }
    }

    public void OnPlayerExit(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player)
        {
            foreach (var monster in m_monsters)
            {
                monster.OnPlayerExit(other);
            }
        }
    }
}
