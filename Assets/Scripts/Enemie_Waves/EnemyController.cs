using UnityEngine;
using UnityEngine.AI;

public class enemyController : MonoBehaviour
{
    private NavMeshAgent nv;
    [SerializeField] private float range = 1000f;
    private Transform player;

    private Transform[] waypoints; // Waypoints que o inimigo deve seguir
    private int currentWaypointIndex = 0; // Índice do waypoint atual

    private bool followingWaypoints = true; // Controla se o inimigo está seguindo waypoints

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void SetWaypoints(Transform[] wp)
    {
        waypoints = wp; // Define os waypoints que o inimigo deve seguir
    }

    void Start()
    {
        nv = GetComponent<NavMeshAgent>();
        if (waypoints.Length > 0)
        {
            nv.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    void Update()
    {
        if (followingWaypoints)
        {
            FollowWaypoints();
        }
        else
        {
            FollowPlayer();
        }
    }

    void FollowWaypoints()
    {
        if (waypoints.Length == 0) return;

        if (!nv.pathPending && nv.remainingDistance < 0.5f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex < waypoints.Length)
            {
                nv.SetDestination(waypoints[currentWaypointIndex].position);
            }
            else
            {
                followingWaypoints = false; // Quando os waypoints terminarem, comece a seguir o jogador
            }
        }
    }

    void FollowPlayer()
    {
        if (Vector3.Distance(player.position, transform.position) < range)
        {
            nv.SetDestination(player.position);
            nv.isStopped = false;
        }
        else
        {
            nv.isStopped = true;
        }
    }
}
