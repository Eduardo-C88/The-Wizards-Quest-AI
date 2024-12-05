using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpell3 : MonoBehaviour
{
	[SerializeField] private LayerMask playerLayer;
	[SerializeField] private LayerMask projectileLayer;
	public GameObject aoeEffect; // The visual effect for the AoE
	
	[Header("Stats")]
	public int damage = 50;
	public float radius = 5;
	public float expansionDuration = 1.0f; // Duration for the AoE to reach full size
	public float knockbackForce = 10f; // Adjust this value to control knockback strength
	
	[Header("Cooldown")]
	public float cooldownDuration = 5.0f;
	
	public bool isReady = true; // Indicates if the spell is ready to be cast
	private float currentRadius = 0f; // To track the expanding radius
	private bool isExpanding = false; // To check if the AoE is expanding
	private HashSet<Player> damagedPlayers = new HashSet<Player>(); // Track damaged players
	
	public void TriggerSpell()
	{
		if(isReady){
			StartCoroutine(ExpandAoE());
			StartCoroutine(Cooldown()); // Start the cooldown coroutine
		}
	}
	
	private IEnumerator ExpandAoE()
	{
		isExpanding = true;
		float startTime = Time.time;
		damagedPlayers.Clear(); // Clear the list of damaged enemies at the start of the attack

		// Instantiate the AoE effect and store the reference
		GameObject aoe = Instantiate(aoeEffect, transform.position, Quaternion.identity);
		Destroy(aoe, expansionDuration + 0.5f); // Destroy the AoE effect after the expansion duration

		// Reset currentRadius for this spell cast
		currentRadius = 0f;

		while (currentRadius < radius)
		{
			// Update the position of the AoE effect to follow the player
			aoe.transform.position = transform.position;

			// Calculate how far along the expansion is
			float elapsed = Time.time - startTime;
			currentRadius = Mathf.Lerp(0, radius, elapsed / expansionDuration);

			// Check for enemies within the current expanding radius
			Collider[] hitColliders = Physics.OverlapSphere(aoe.transform.position, currentRadius, playerLayer);
			Collider[] projectileColliders = Physics.OverlapSphere(aoe.transform.position, currentRadius, projectileLayer);
	
			foreach (Collider c in hitColliders)
			{
				Player player = c.GetComponent<Player>();

				if (player != null && !damagedPlayers.Contains(player))
				{
					// Check if there is a clear line-of-sight to the player
					Vector3 directionToPlayer = (player.transform.position - aoe.transform.position).normalized;
					Ray ray = new Ray(aoe.transform.position, directionToPlayer);
					RaycastHit hit;

					// Perform the raycast and check if it hits something other than the player
					if (Physics.Raycast(ray, out hit, currentRadius, ~playerLayer))
					{
						if (hit.collider.gameObject != player.gameObject)
						{
							// If the ray hits something other than the player, skip this player
							Debug.Log("Blocked by: " + hit.collider.name);
							continue;
						}
					}

					// Apply damage and knockback since there's a clear line-of-sight
					player.TakeDamage(damage);

					// Calculate direction for knockback
					Vector3 knockbackDirection = (player.transform.position - aoe.transform.position).normalized;
					player.ApplyKnockback(knockbackDirection, knockbackForce);

					damagedPlayers.Add(player);
				}
			}
			
			foreach (Collider c in projectileColliders)
			{
				ProjectileAddon projectile = c.GetComponent<ProjectileAddon>();
				ProjectileSpell1 projectile1 = c.GetComponent<ProjectileSpell1>();

				if (projectile != null) 
				{
					Destroy(projectile.gameObject);
					Debug.Log("Projectile destroyed");
				}
				if (projectile1 != null) 
				{
					Destroy(projectile1.gameObject);
					Debug.Log("Projectile destroyed");

				}
			}

			// Wait until the next frame
			yield return null;
		}

		// Reset the values after the expansion is complete
		currentRadius = 0f; // Reset radius to allow for the next cast
		isExpanding = false; // Set isExpanding back to false to allow the skill to be used again
	}
	
	// Coroutine to handle cooldown
	private IEnumerator Cooldown()
	{
		isReady = false; // Set the spell to not ready
		yield return new WaitForSeconds(cooldownDuration); // Wait for the cooldown duration
		isReady = true; // Set the spell to ready again
	}
	
	private void OnDrawGizmos()
	{
		if (isExpanding) // Only draw the gizmo when expanding
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(transform.position, currentRadius);
		}
	}
}
