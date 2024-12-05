using UnityEngine.AI;
using UnityEngine;

public class FleeState : IState
{
    private Enemy_NavMesh enemy;
    private GameObject player;
    private float fleeDuration = 5f; // How long to flee
    private float fleeStartTime;

    public FleeState(Enemy_NavMesh enemy)
    {
        this.enemy = enemy;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Enter()
    {
        Debug.Log("Entering Flee State");
        fleeStartTime = Time.time;

        // Begin fleeing
        FleeFromPlayer();
    }

    public void Execute()
    {
        if (enemy.BasicEnemy.IsDead)
        {
            return; // Stop execution if the enemy is dead
        }

        // Check if flee duration has elapsed
        if (Time.time >= fleeStartTime + fleeDuration)
        {
            // If health has recovered, return to normal behavior
            if (enemy.BasicEnemy.Health > 50)
            {
                enemy.StateMachine.ChangeState(new RunState(enemy));
            }
            else
            {
                // Continue fleeing
                FleeFromPlayer();
            }
            return;
        }

        // Ensure the enemy keeps fleeing
        FleeFromPlayer();
    }

    public void Exit()
    {
        Debug.Log("Exiting Flee State");
    }

    private void FleeFromPlayer()
    {
        Vector3 fleeDirection = (enemy.transform.position - player.transform.position).normalized;
        Vector3 fleeTarget = enemy.transform.position + fleeDirection * 10f; // Run 10 units away
        enemy.GetComponent<NavMeshAgent>().SetDestination(fleeTarget);
    }
}
