using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public BasicEnemyAgent enemyAgent; // Reference to the agent
    public float maxTime = 30f; // Max time before resetting the environment

    private float timer;

    private void Update()
    {
        // Increment timer
        timer += Time.deltaTime;

        // Reset environment if time exceeds maxTime
        if (timer >= maxTime)
        {
            ResetEnvironment();
        }
    }

    public void ResetEnvironment()
    {
        timer = 0f; // Reset timer
        enemyAgent.EndEpisode(); // Trigger reset on the agent
    }
}
