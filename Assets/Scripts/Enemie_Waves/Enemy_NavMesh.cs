using UnityEngine;
using UnityEngine.AI;

public class Enemy_NavMesh : MonoBehaviour
{
    public StateMachine StateMachine { get; private set; }
    public BasicEnemy BasicEnemy { get; private set; }
    private NavMeshAgent agent;
    private GameObject player;

    void Start()
    {
        BasicEnemy = GetComponent<BasicEnemy>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");

        // Initialize the FSM
        StateMachine = new StateMachine();
        StateMachine.ChangeState(new RunState(this));
    }

    void Update()
    {
        StateMachine.Update();
    }

    // Movement logic
    public void MoveToPlayer()
    {
        if (!BasicEnemy.IsDead)
        {
            agent.SetDestination(player.transform.position);
        }
    }

    public void StopMovement()
    {
        agent.isStopped = true;
    }

    // Attack logic
    public void PerformAttack()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < 2f)
        {
            Debug.Log("Attacking the player!");
        }
    }

    // Flee logic
    public void FleeFromPlayer()
    {
        Vector3 fleeDirection = (transform.position - player.transform.position).normalized;
        Vector3 fleeTarget = transform.position + fleeDirection * 10f;
        agent.SetDestination(fleeTarget);
    }
}
