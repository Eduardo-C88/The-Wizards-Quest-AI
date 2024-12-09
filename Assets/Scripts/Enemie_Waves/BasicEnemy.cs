using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : MonoBehaviour
{
	[Header("Stats")]
	public int health = 100;
	public int damage = 10;
	private int baseHealth;

	private bool isDead = false;
	private WaveManager waveManager;
	
	[SerializeField] private TextMeshPro healthText;
	
	public AudioClip deathSound;
	private AudioSource audioSource;
	
	private void Start()
	{
		waveManager = FindObjectOfType<WaveManager>();
		baseHealth = health;
		audioSource = GetComponent<AudioSource>();

	}

	public bool IsDead => isDead;
	
	private void Update()
	{
		HealthTextUpdate();
	}

	// Method for taking damage
	public void TakeDamage(int damage)
	{
		if (isDead) return; // Prevent further actions if already dead

		health -= damage;

		if (health <= 0)
		{
			Die();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.GetComponent<Player>() != null)
		{
			Player player = collision.gameObject.GetComponent<Player>();
			player.TakeDamage(damage);
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
		
		// Toca o som de morte
		if (deathSound != null)
		{
			audioSource.PlayOneShot(deathSound);
		}
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
	
	private void HealthTextUpdate()
	{
		if(healthText != null)
		{
			healthText.text = health + "/" + baseHealth;
			
			Vector3 direction = healthText.transform.position - Camera.main.transform.position;
			Quaternion rotation = Quaternion.LookRotation(direction);
			healthText.transform.rotation = rotation;
		}
	}
}
