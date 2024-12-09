using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class BasicEnemyAgent : Agent
{
    public LayerMask enemyLayer;
    public float moveSpeed = 5f;
    public float rotationSpeed = 200f;
    public float proximityPenaltyRange = 1.5f;
    public float proximityToPlayerRange = 2.0f;
    public bool showGizmos = true;

    private Rigidbody rb;
    private Transform player;
    private Rigidbody playerRb;
    private Vector3 originalSpawnPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            playerRb = playerObject.GetComponent<Rigidbody>();
            Debug.Log("Player successfully found and reference assigned.");
        }
        else
        {
            Debug.LogError("No GameObject with tag 'Player' found in the scene.");
        }

        originalSpawnPosition = transform.position;
    }

    public override void OnEpisodeBegin()
    {
        // Reset the position and velocity
        //transform.position = originalSpawnPosition;
        //rb.velocity = Vector3.zero;

        // Stop player movement if needed
        
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (player == null)
        {
            Debug.LogError("Player reference is null in CollectObservations.");
            return;
        }

        // Add normalized relative position to the player
        Vector3 relativePosition = player.position - transform.position;
        sensor.AddObservation(relativePosition.normalized);

        // Add normalized distance to the player
        sensor.AddObservation(Mathf.Clamp01(relativePosition.magnitude / 50f));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Read the continuous actions
        float moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float moveZ = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        // Prevent NaN values from causing issues
        if (float.IsNaN(moveX) || float.IsNaN(moveZ))
        {
            Debug.LogWarning("NaN action detected; clamping to zero.");
            moveX = 0f;
            moveZ = 0f;
        }

        // Create the movement direction vector
        Vector3 targetDirection = new Vector3(moveX, 0, moveZ).normalized;

        // Force the agent to always move at the defined moveSpeed
        Vector3 targetVelocity = targetDirection * moveSpeed;

        // Apply movement direction directly to the Rigidbody velocity
        rb.velocity = targetVelocity;

        // Rotate the agent to face the direction it's moving
        if (targetDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Provide heuristic actions for manual testing or debugging
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        // RequestDecision lets the agent decide its next action
        RequestDecision();
    }

    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, proximityToPlayerRange);

            if (player != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, player.position);
            }
        }
    }
}
