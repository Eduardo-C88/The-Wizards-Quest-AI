using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int health = 100;
    public int maxHealth = 100;
    //private TrainingEnvironment environment;

    private void Start()
    {
        //environment = FindObjectOfType<TrainingEnvironment>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"Player health: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    public void ResetPlayer()
    {
        health = maxHealth;

        // Reset the player's position and ensure they are active
        transform.position = new Vector3(60, 4.5f, 65);
        transform.rotation = Quaternion.identity;
        gameObject.SetActive(true);

        Debug.Log("Player has been reset.");
    }

    private void Die()
    {
        //if (environment != null)
        //{
        //    environment.PlayerDiedCondition(); // Notify the environment of the player's death
        //}

        //// Deactivate the player to simulate "death"
        //gameObject.SetActive(false);
        //Debug.Log("Player died.");
    }

    private void Update()
    {
        // Prevent the player from falling below a certain point
        if (transform.position.y < 0)
        {
            transform.position = new Vector3(60, 4.5f, 65);
        }

        // Keep the player upright by resetting unintended rotations
        Vector3 currentRotation = transform.rotation.eulerAngles;
        if (currentRotation.x > 90f && currentRotation.x < 270f)
        {
            transform.rotation = Quaternion.Euler(0, currentRotation.y, 0);
        }
    }
}