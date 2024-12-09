//using UnityEngine;

//public class BasicEnemyML : MonoBehaviour
//{
//    public int maxHealth = 10;
//    public int health;
//    public int damage = 10;
//    private bool isDead = false;

//    private Transform player;
//    private WaveManagerML waveManager;

//    private void Start()
//    {
//        InitializeEnemy();
//    }

//    public void InitializeEnemy()
//    {
//        health = maxHealth;
//        isDead = false;

//        // Find the player
//        player = GameObject.FindGameObjectWithTag("Player")?.transform;

//        waveManager = FindObjectOfType<WaveManagerML>();
//        if (waveManager == null)
//        {
//            Debug.LogError("WaveManagerML not found in the scene!");
//        }
//    }

//    public void TakeDamage(int damage)
//    {
//        if (isDead) return;

//        health -= damage;

//        if (health <= 0)
//        {
//            Die();
//        }
//    }

//    private void Die()
//    {
//        if (isDead) return;

//        isDead = true;

//        WaveManagerML waveManager = FindObjectOfType<WaveManagerML>();
//        if (waveManager != null)
//        {
//            waveManager.EnemyDefeated(gameObject);
//        }

//        Destroy(gameObject);
//    }

//    public void Respawn(Transform spawnPoint)
//    {
//        InitializeEnemy();
//        transform.position = spawnPoint.position;
//        transform.rotation = spawnPoint.rotation;
//        gameObject.SetActive(true);
//    }

//    // Detect collision with the player and apply damage
//    private void OnCollisionEnter(Collision collision)
//    {
//        if (collision.gameObject.CompareTag("Player"))
//        {
//            // Apply damage to the player when colliding
//            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
//            if (playerStats != null)
//            {
//                playerStats.TakeDamage(damage);  // Apply the enemy's damage to the player
//                Debug.Log("Player hit by enemy!");
//            }
//        }
//    }
//}