using BehaviorTree;
using UnityEngine;

public class CheckLoS : Node
{
    private Transform enemyTransform;  // Transform of the entity (the boss)
    private Transform playerTransform; // Reference to the player's transform
    private float maxRange;            // Maximum range for checking (already handled by the tree)

    public CheckLoS(Transform enemyTransform, Transform playerTransform, float maxRange)
    {
        this.enemyTransform = enemyTransform;
        this.playerTransform = playerTransform;
        this.maxRange = maxRange;
    }

    public override NodeState Evaluate()
    {
        // If player reference is missing, return failure (can't check LoS)
        if (playerTransform == null) 
        {
            state = NodeState.Failure;
            return state;
        }

        // Cast a ray from the enemy to the player
        Ray ray = new Ray(enemyTransform.position, playerTransform.position - enemyTransform.position);
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, maxRange))
        {
            // Check if the ray hits the player directly
            if (hit.collider.CompareTag("Player"))
            {
                state = NodeState.Success;  // Line of sight is clear
            }
            else
            {
                state = NodeState.Failure;  // Hit something else (wall, obstacle)
            }
        }
        else
        {
            state = NodeState.Failure;  // Ray did not hit anything (something blocking LoS)
        }

        return state;
    }
}
