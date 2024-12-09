using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MiniBossController : MonoBehaviour
{
	private NavMeshAgent nv;
	[SerializeField] private float range = 1000f;
	private Transform player;

	[Header("Mini Boss Stats")]
	public int health = 500;
	public int damage = 50;
	public float knockbackForce = 20f;
	private bool isDead = false;
	public bool skillActive = false;

	private Shield shield; // Referência ao escudo
	private int baseShieldDurability;
	private PortalManager portalManager;
	
	private int baseHealth;
	[SerializeField] private TextMeshPro healthText;
	
	public AudioClip deathSound;  // Som de morte do MiniBoss
	private AudioSource audioSource;

	void Start()
	{
		nv = GetComponent<NavMeshAgent>();
		player = GameObject.FindGameObjectWithTag("Player").transform;
		shield = GetComponentInChildren<Shield>(); // Encontra o escudo no objeto filho
		portalManager = FindObjectOfType<PortalManager>();
		audioSource = GetComponent<AudioSource>(); // Referência ao AudioSource

		baseHealth = health;
		baseShieldDurability = shield.shieldDurability;
	}

	void Update()
	{
		HealthTextUpdate();
		
		if (!isDead && Vector3.Distance(player.position, transform.position) < range)
		{
			nv.SetDestination(player.position);
		}
	}

	public void TakeDamage(int damage)
	{
		if (shield != null)
		{
			// Se o escudo está ativo, o dano é aplicado a ele
			shield.TakeDamage(damage);
			if (shield.shieldDurability <= 0)
			{
				shield = null; // Remove a referência ao escudo, pois ele foi destruído
			}
		}
		else
		{
			// Se o escudo foi destruído, o mini boss recebe dano
			health -= damage;
			Debug.Log("Mini Boss took damage. Remaining health: " + health);

			if (health <= 0)
			{
				isDead = true;
				Debug.Log("Mini Boss defeated!");
				// Toque o som de morte antes de destruir o objeto
				if (deathSound != null && audioSource != null)
				{
					audioSource.PlayOneShot(deathSound);
				}
				portalManager.SpawnPortal();
				Destroy(gameObject, 1f);


			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.GetComponent<Player>() != null)
		{
			Player player = collision.gameObject.GetComponent<Player>();
			player.TakeDamage(damage);

			Vector3 knockbackDirection = collision.transform.position - transform.position;
			player.ApplyKnockback(knockbackDirection, knockbackForce);
		}
	}
	
	public void ApplySlow(float slowAmount, float duration)
	{
		StartCoroutine(Slow(slowAmount, duration));
	}

	private IEnumerator Slow(float slowAmount, float duration)
	{
		NavMeshAgent agent = GetComponent<NavMeshAgent>();
		float originalSpeed = agent.speed;

		agent.speed *= slowAmount;

		float remainingTime = duration;
		while (remainingTime > 0)
		{
			Debug.Log("Slow effect active. Remaining time: " + remainingTime + " seconds.");
			yield return new WaitForSeconds(1f);
			remainingTime -= 1f;
		}

		agent.speed = originalSpeed;
		Debug.Log("Slow effect ended. Speed reset to normal.");
	}
	
	public void PauseMovement(bool stop)
	{
		if (nv != null)
		{
			nv.isStopped = stop; // Stops pathfinding and movement
			nv.velocity = Vector3.zero; // Clears current velocity
		}
	}
	
	private void HealthTextUpdate()
	{
		if(healthText != null)
		{
			if(shield == null)
			{
				healthText.text = $"{health}/{baseHealth}";
			}
			else{
				healthText.text = $"({shield?.shieldDurability})/({baseShieldDurability})\n{health}/{baseHealth}";
			}
			
			Vector3 direction = healthText.transform.position - Camera.main.transform.position;
			Quaternion rotation = Quaternion.LookRotation(direction);
			healthText.transform.rotation = rotation;
		}
	}
}