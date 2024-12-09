using UnityEngine;

public class AttackState : IState
{
    private Enemy_NavMesh enemy;
    private GameObject player;
    private float attackCooldown = 1.5f; // Time between attacks
    private float lastAttackTime;

    public AttackState(Enemy_NavMesh enemy)
    {
        this.enemy = enemy;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Enter()
    {
        Debug.Log("Entering Attack State");
        lastAttackTime = Time.time - attackCooldown; // Allow an immediate attack
    }

    public void Execute()
    {
        if (enemy.BasicEnemy.IsDead)
        {
            return; // Stop execution if the enemy is dead
        }

        // Check if player is still in attack range
        if (Vector3.Distance(enemy.transform.position, player.transform.position) > 2f)
        {
            enemy.StateMachine.ChangeState(new RunState(enemy));
            return;
        }

        // Perform an attack if the cooldown has passed
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            enemy.PerformAttack(); // Trigger attack logic
            lastAttackTime = Time.time;
        }

        if (enemy.BasicEnemy.health <= 50)
        {
            Debug.Log("Health low, transitioning to FleeState.");
            enemy.StateMachine.ChangeState(new FleeState(enemy));
        }
    }

    public void Exit()
    {
        Debug.Log("Exiting Attack State");
    }
}
