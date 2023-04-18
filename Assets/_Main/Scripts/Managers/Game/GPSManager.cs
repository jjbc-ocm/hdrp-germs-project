using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GPSManager : Singleton<GPSManager>
{
    [SerializeField]
    private GameObject destinationMarker;

    [SerializeField]
    private LineRenderer pathMarker;

    private NavMeshAgent agent;

    #region Unity

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        var player = PlayerManager.Mine;

        if (player == null) return;

        // Put GPS starting marker on player's position
        transform.position = player.transform.position;

        // Set destination based on these conditions
        if (player.Stat.HasKey)
        {
            SetDestination(GameManager.Instance.GetBase(1 - player.GetTeam()).transform.position);
        }
        else if (player.Stat.HasChest())
        {
            SetDestination(GameManager.Instance.GetBase(player.GetTeam()).transform.position);
        }
        else
        {
            ClearDestination();
        }

        // Draw path, if there is
        if (agent.hasPath && destinationMarker.activeSelf)
        {
            DrawPath(player.transform.position);
        }
    }

    #endregion

    #region Private

    private void SetDestination(Vector3 position)
    {
        agent.SetDestination(position);

        destinationMarker.SetActive(true);

        destinationMarker.transform.position = position;


    }

    private void ClearDestination()
    {
        agent.isStopped = true;

        destinationMarker.SetActive(false);

        pathMarker.positionCount = 0;
    }

    private void DrawPath(Vector3 startPosition)
    {
        pathMarker.positionCount = agent.path.corners.Length;

        pathMarker.SetPosition(0, startPosition);

        if  (agent.path.corners.Length < 2)
        {
            return;
        }

        for (var i = 1; i < agent.path.corners.Length; i++)
        {
            var position = agent.path.corners[i];

            pathMarker.SetPosition(i, position);
        }
    }

    #endregion
}
