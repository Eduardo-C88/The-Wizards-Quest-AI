using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossRangedEnemy : MonoBehaviour
{
	public GameObject projectilePrefab;      // Prefab for the projectile (e.g., snowball)
	public Transform shootingPoint;          // Point from where the projectile will be shot
	public float shootingRange = 10f;        // Distance to start shooting
	public float minShootingDistance = 2f;   // Minimum distance to stop shooting
	public float fireRate = 2f;              // Time between shots
	public float launchForce = 10f;          // Projectile speed
	public float slightArc = 0.6f;           // Small vertical arc for projectile
	private float nextFireTime = 0f;

	private NavMeshAgent agent;
	private Transform player;
	private MiniBossController miniBossController;

	public Animator anim;
	
	public AudioClip shootSound; // Som do disparo do projétil
	private AudioSource audioSource;	

	[Header("Enemy Stats")]
	public float range = 1000f;              // Maximum chasing distance to the player

	void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		player = GameObject.FindGameObjectWithTag("Player").transform;
		miniBossController = GetComponent<MiniBossController>();
		audioSource = GetComponent<AudioSource>();

		// Ensure the NavMeshAgent is active and set speed
		if (agent != null)
		{
			agent.isStopped = false;
			agent.speed = 3.5f; // Base speed of the enemy
		}
	}

	void Update()
	{
		if (player == null || agent == null) return;

		float distanceToPlayer = Vector3.Distance(transform.position, player.position);

		// If the player is within maximum chase range and no skill is active
		if (distanceToPlayer <= range && !miniBossController.skillActive)
		{
			agent.SetDestination(player.position);

			// If the player is within shooting range but outside the minimum distance
			if (distanceToPlayer <= shootingRange && distanceToPlayer > minShootingDistance)
			{
				// Shoot and animate if cooldown has passed
				if (Time.time >= nextFireTime)
				{
					nextFireTime = Time.time + 1f / fireRate;
					ShootProjectile();

					// Trigger the attack animation
					anim.SetTrigger("RangedAttack");
				}
			}
		}
		else
		{
			// Stop the agent when out of range
			agent.isStopped = true;
		}
	}

	private void ShootProjectile()
	{
		// Instantiate and launch the projectile
		GameObject projectile = Instantiate(projectilePrefab, shootingPoint.position, Quaternion.identity);
		Rigidbody rb = projectile.GetComponent<Rigidbody>();

		// Offset the player's position to aim at the head
		Vector3 playerHead = player.position;

		if (rb.useGravity == true)
		{
			playerHead = player.position + new Vector3(0, 1.0f, 0); // Aim slightly upward if gravity is enabled
		}

		// Calculate direction with a slight arc
		Vector3 direction = (playerHead - shootingPoint.position).normalized;

		// Combine horizontal direction with a slight vertical arc
		Vector3 launchVelocity = direction * launchForce + Vector3.up * slightArc;
		rb.velocity = launchVelocity; // Set initial velocity for the projectile
		
		// Toque o som de lançamento do projétil
		if (shootSound != null && audioSource != null)
		{
			audioSource.PlayOneShot(shootSound);
		}
	}
}
