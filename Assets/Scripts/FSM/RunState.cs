using UnityEngine;

public class RunState : IState
{
    private Enemy_NavMesh enemy;
    private GameObject player;

    public RunState(Enemy_NavMesh enemy)
    {
        this.enemy = enemy;
        this.player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Enter()
    {
        Debug.Log("Entering RunState");
    }

    public void Execute()
    {
        // Move towards the player
        
        enemy.MoveToPlayer();

        // Transition conditions:
        // If the player is close enough, move to AttackState
        if (Vector3.Distance(enemy.transform.position, player.transform.position) <= 2f)
        {
            Debug.Log("Player in range, transitioning to AttackState.");
            enemy.StateMachine.ChangeState(new AttackState(enemy));
        }

        // If health drops below a certain threshold, transition to FleeState
        if (enemy.BasicEnemy.health <= 50)
        {
            Debug.Log("Health low, transitioning to FleeState.");
            enemy.StateMachine.ChangeState(new FleeState(enemy));
        }
    }

    public void Exit()
    {
        Debug.Log("Exiting RunState");
    }
}
