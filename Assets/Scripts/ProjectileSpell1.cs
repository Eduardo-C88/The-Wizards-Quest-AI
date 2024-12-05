using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpell1 : MonoBehaviour
{
	[SerializeField] private LayerMask enemyLayer;
	
	[HideInInspector]public Player player;

	[Header("Stats")]
	public int damage = 50;
	public int exploDamage = 40;
	public float radius = 0.012f;
	public float expansionDuration = 1.0f; // Duration for the AoE to reach full size
	public float lifespan = 5f; // Lifespan of projectile in seconds

	[Header("References")]
	public GameObject aoeProjectilePrefab; // Reference to the AoE projectile prefab

	private float currentRadius = 0f; // To track the expanding radius
	private bool isExpanding = false; // To check if the AoE is expanding
	private HashSet<BasicEnemy> damagedEnemies = new HashSet<BasicEnemy>(); // Track damaged BasicEnemies
	private HashSet<MiniBossController> damagedBosses = new HashSet<MiniBossController>(); // Track damaged BossEnemies
	private HashSet<BossController> damagedFinalBoss = new HashSet<BossController>(); // Track damaged FinalBoss

	private Rigidbody rb;
	private bool targetHit;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		
		if (player == null)
		{
			player = FindObjectOfType<Player>();
			if (player == null)
			{
				Debug.LogError("Player not found! Make sure there is a Player object in the scene.");
			}else{
				Debug.Log("Player found!");
			}
		}
		
		StartCoroutine(SelfDestructTimer()); // Start self-destruct timer
		Destroy(gameObject, 4f);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag != "Bullet" && collision.gameObject.tag != "Player" && !targetHit)
		{
			if (targetHit)
				return;

			targetHit = true;

			int modDamage = Mathf.RoundToInt(damage * player.damageMultiplier); // Apply damage boost

			// Apply direct damage to the hit BasicEnemy, if applicable
			BasicEnemy hitEnemy = collision.gameObject.GetComponent<BasicEnemy>();
			if (hitEnemy != null)
			{
				hitEnemy.TakeDamage(modDamage); // Direct damage on impact
			}

			// Similarly for MiniBossController:
			MiniBossController hitBoss = collision.gameObject.GetComponent<MiniBossController>();
			if (hitBoss != null)
			{
				hitBoss.TakeDamage(modDamage); // Direct damage on impact
			}
			
			// Similarly for BossController:
			BossController hitFinalBoss = collision.gameObject.GetComponent<BossController>();
			if (hitFinalBoss != null)
			{
				hitFinalBoss.TakeDamage(modDamage); // Direct damage on impact
			}

			// Start the AoE explosion effect regardless of what was hit
			StartCoroutine(ExpandAoE());

			rb.detectCollisions = false;

			// Disable further physics interactions
			rb.isKinematic = true;
			Debug.Log("Projectile hit something and triggered an explosion!");
		}
	}

	private IEnumerator ExpandAoE()
	{
		isExpanding = true;
		float startTime = Time.time;
		damagedEnemies.Clear(); // Clear the list of damaged enemies at the start of the attack
		damagedBosses.Clear(); // Clear the list of damaged bosses at the start of the attack
		damagedFinalBoss.Clear(); // Clear the list of damaged final boss at the start of the attack

		// Instantiate the AoE projectile
		var aoeProjectileObj = Instantiate(aoeProjectilePrefab, transform.position, Quaternion.identity);

		if (aoeProjectileObj != null)
		{
			Debug.Log("AoE projectile instantiated at: " + transform.position);
		}
		else
		{
			Debug.LogError("Failed to instantiate AoE projectile!");
		}

		while (currentRadius < radius)
		{
			// Calculate how far along the expansion is
			float elapsed = Time.time - startTime;
			currentRadius = Mathf.Lerp(0, radius, elapsed / expansionDuration);

			// Check for enemies (BasicEnemy) within the current expanding radius
			Collider[] hitColliders = Physics.OverlapSphere(aoeProjectileObj.transform.position, currentRadius, enemyLayer);
			int modDamage = Mathf.RoundToInt(damage * player.damageMultiplier); // Calculate damage once per expansion step
			
			foreach (Collider c in hitColliders)
			{
				BasicEnemy enemy = c.GetComponent<BasicEnemy>();
				MiniBossController boss = c.GetComponent<MiniBossController>();
				BossController finalBoss = c.GetComponent<BossController>();

				if (enemy != null && !damagedEnemies.Contains(enemy))
				{
					// Apply damage to BasicEnemy and add to damaged list
					enemy.TakeDamage(modDamage);
					damagedEnemies.Add(enemy);
				}
				else if (boss != null && !damagedBosses.Contains(boss))
				{
					// Apply damage to MiniBossController and add to damaged list
					boss.TakeDamage(modDamage);
					damagedBosses.Add(boss);
				}
				else if (finalBoss != null && !damagedFinalBoss.Contains(finalBoss))
				{
					// Apply damage to BossController and add to damaged list
					finalBoss.TakeDamage(modDamage);
					damagedFinalBoss.Add(finalBoss);
				}
			}

			// Wait until the next frame
			yield return null;
		}

		// Explosion completed, now destroy the AoE projectile
		Destroy(aoeProjectileObj);

		// Reset the values after the expansion is complete
		currentRadius = 0f;
		isExpanding = false;
	}

	private void OnDrawGizmos()
	{
		if (isExpanding) // Only draw the gizmo when expanding
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, currentRadius);
		}
	}

	// Coroutine to destroy projectile after a set lifespan if it doesn't hit anything
	private IEnumerator SelfDestructTimer()
	{
		yield return new WaitForSeconds(lifespan); // Wait for the specified lifespan
		if (!targetHit) // Only destroy if no collision has occurred
		{
			StartCoroutine(ExpandAoE()); // Trigger AoE explosion before self-destruct
			Debug.Log("Projectile self-destructed after timeout.");
		}
	}
}
