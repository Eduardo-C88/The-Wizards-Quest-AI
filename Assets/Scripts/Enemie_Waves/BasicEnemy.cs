using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : MonoBehaviour
{
    [Header("Stats")]
    public int Health = 100;
    public int Damage = 10;

    private bool isDead = false;
    private WaveManager waveManager;

    private void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
    }

    public bool IsDead => isDead;

    // Method for taking damage
    public void TakeDamage(int damage)
    {
        if (isDead) return; // Prevent further actions if already dead

        Health -= damage;

        if (Health <= 0)
        {
            Die();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            Player player = collision.gameObject.GetComponent<Player>();
            player.TakeDamage(Damage);
        }
        else
        {
            return;
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Enemy is dead!");

        // Notify WaveManager
        waveManager?.EnemyKilled(gameObject);

        // Trigger destruction after a delay
        Destroy(gameObject, 1f);
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        ApplySlow(0f, delay);
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    // Apply a knockback effect
    public void ApplyKnockback(Vector3 direction, float force)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(direction.normalized * force, ForceMode.Impulse);
        }
    }

    // Apply a slow effect
    public void ApplySlow(float slowAmount, float duration)
    {
        StartCoroutine(Slow(slowAmount, duration));
    }

    private IEnumerator Slow(float slowAmount, float duration)
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent == null) yield break;

        float originalSpeed = agent.speed;
        agent.speed *= slowAmount;

        yield return new WaitForSeconds(duration);

        agent.speed = originalSpeed;
    }
}
