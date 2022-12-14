using System.Collections;
using System.Collections.Generic;
using TanksMP;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GPSManager : MonoBehaviour
{
    public static GPSManager Instance;

    [SerializeField]
    private GameObject destinationMarker;

    [SerializeField]
    private LineRenderer pathMarker;

    private NavMeshAgent agent;

    void Awake()
    {
        Instance = this;

        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        var player = Player.Mine;

        if (player != null)
        {
            transform.position = player.transform.position;
        }

        if (Vector3.Distance(agent.destination, transform.position) <= agent.stoppingDistance)
        {
            destinationMarker.SetActive(false); // TODO: actually this only happen when done drooping chest
        }
        else if (agent.hasPath && destinationMarker.activeSelf)
        {
            DrawPath(player.transform.position);
        }
    }

    public void SetDestination(Vector3 position)
    {
        agent.SetDestination(position);

        destinationMarker.SetActive(true);

        destinationMarker.transform.position = position;


    }

    public void ClearDestination()
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
}
