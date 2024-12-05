using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpell1 : MonoBehaviour
{
	[Header("References")]
	public GameObject initialProjectilePrefab; // Prefab for the first projectile
	public GameObject aoeProjectilePrefab; // Prefab for the AoE projectile
	public Transform firePoint;
	[HideInInspector] public Transform player; // Reference to the player

	[Header("Projectile Stats")]
	public float maxRange = 50;
	public float minRange = 10;
	public float projectileSpeed = 30;

	[Header("Cooldown")]
	public float cooldownDuration = 5.0f;
	public bool isReady = true; // Indicates if the spell is ready to be cast

	void Start()
	{
		if (initialProjectilePrefab == null)
		{
			Debug.LogError("Initial Projectile Prefab is not assigned!");
		}
		if (aoeProjectilePrefab == null)
		{
			Debug.LogError("AOE Projectile Prefab is not assigned!");
		}

		player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player object
		Debug.Log("Player found: " + player.name + " at " + player.position);
	}


	// void Update()
	// {
	// 	// Check if the spell is ready to fire and the player is within range
	// 	if (isReady && PlayerInRange())
	// 	{
	// 		ShootProjectile();
	// 		StartCoroutine(Cooldown()); // Start the cooldown coroutine
	// 	}
	// }
	
	public void TriggerSpell()
	{
		if(isReady){
			ShootProjectile();
			StartCoroutine(Cooldown()); // Start the cooldown coroutine
		}
	}

	void ShootProjectile()
	{
		if (player != null)
		{
			// Offset the player's position to aim at the head
			Vector3 playerHead = player.position + new Vector3(0, 1.0f, 0);
			
			// Calculate the direction from the enemy to the player
			Vector3 direction = (playerHead - firePoint.position).normalized;

			// Instantiate and launch the projectile
			InstantiateProjectile(direction);
		}
	}

	void InstantiateProjectile(Vector3 direction)
	{
		// Instantiate the initial projectile
		var projectileObj = Instantiate(initialProjectilePrefab, firePoint.position, Quaternion.identity);
		projectileObj.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
	}

	// Coroutine to handle cooldown
	private IEnumerator Cooldown()
	{
		isReady = false; // Set the spell to not ready
		yield return new WaitForSeconds(cooldownDuration); // Wait for the cooldown duration
		isReady = true; // Set the spell to ready again
	}
}
