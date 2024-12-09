using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public GameObject projectilePrefab; // The projectile prefab to shoot
    public Transform firePoint; // The point where projectiles are spawned
    public Transform enemyTarget; // Reference to the enemy
    public float fireInterval = 1f; // Time between shots

    private float fireTimer = 0f;

    private void Update()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireInterval && enemyTarget != null)
        {
            ShootAtEnemy();
            fireTimer = 0f;
        }
    }

    private void ShootAtEnemy()
    {
        // Calculate the direction toward the enemy
        Vector3 directionToEnemy = (enemyTarget.position - firePoint.position).normalized;

        // Instantiate and orient the projectile
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(directionToEnemy));

        // Optionally tag the projectile as "Obstacle"
        projectile.tag = "Obstacle";
    }
}
