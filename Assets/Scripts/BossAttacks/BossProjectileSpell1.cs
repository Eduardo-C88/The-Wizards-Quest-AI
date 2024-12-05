using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectileSpell1 : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;

	[Header("Stats")]
	public int damage = 50;
	public int exploDamage = 40;
	public float radius = 2.0f;
	public float expansionDuration = 1.0f; // Duration for the AoE to reach full size
	public float lifespan = 5f; // Lifespan of projectile in seconds

	[Header("References")]
	public GameObject aoeProjectilePrefab; // Reference to the AoE projectile prefab

	private float currentRadius = 0f; // To track the expanding radius
	private bool isExpanding = false; // To check if the AoE is expanding
	private HashSet<Player> damagedPlayer = new HashSet<Player>(); // Track damaged BasicEnemies

	private Rigidbody rb;
	private bool targetHit;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		
		StartCoroutine(SelfDestructTimer()); // Start self-destruct timer
		Destroy(gameObject, 4f);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag != "Bullet" && collision.gameObject.tag != "Enemy" && !targetHit)
		{
			if (targetHit)
				return;

			targetHit = true;


			// Apply direct damage to the hit BasicEnemy, if applicable
			Player hitPlayer = collision.gameObject.GetComponent<Player>();
			if (hitPlayer != null)
			{
				hitPlayer.TakeDamage(damage); // Direct damage on impact
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
		damagedPlayer.Clear(); // Clear the list of damaged players at the start of the attack

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

			// Check for players within the current expanding radius
			Collider[] hitColliders = Physics.OverlapSphere(aoeProjectileObj.transform.position, currentRadius, playerLayer);

			foreach (Collider c in hitColliders)
			{
				Player player = c.GetComponent<Player>();

				if (player != null && !damagedPlayer.Contains(player))
				{
					// Line-of-sight check: Raycast to ensure no obstacles are in the way
					Vector3 directionToPlayer = (player.transform.position - aoeProjectileObj.transform.position).normalized;
					Ray ray = new Ray(aoeProjectileObj.transform.position, directionToPlayer);
					RaycastHit hit;

					// Perform the raycast and check if it hits something other than the player
					if (Physics.Raycast(ray, out hit, currentRadius, ~playerLayer))
					{
						if (hit.collider.gameObject != player.gameObject)
						{
							// If the ray hits something other than the player, skip this player
							Debug.Log("AoE blocked by: " + hit.collider.name);
							continue;
						}
					}

					// Apply damage to the player since there's a clear line-of-sight
					player.TakeDamage(exploDamage);
					damagedPlayer.Add(player);
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
			Gizmos.color = Color.green;
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
