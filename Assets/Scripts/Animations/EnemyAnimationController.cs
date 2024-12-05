
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
	public Animator anim; // Reference to the Animator component
	private Transform playerTransform; // Reference to the player's transform
	private BasicEnemy Enemy;
	

	void Start()
	{
		anim = GetComponent<Animator>();
		// Find the player by tag (or assign manually in the inspector)
		GameObject player = GameObject.FindWithTag("Player");
		if (player != null)
		{
			playerTransform = player.transform;
		}
		else
		{
			Debug.LogError("Player not found! Ensure the Player has the correct tag.");
		}

		if (Enemy == null)
		{
			Enemy = GetComponent<BasicEnemy>();
			if (Enemy == null)
			{
				Debug.LogError("BasicEnemy component not found! Ensure it is attached to the same GameObject.");
			}
		}
		
	}

	void Update()
	{
		if (playerTransform == null) return;

		// Calculate the distance between the player and the enemy
		float distance = Vector3.Distance(transform.position, playerTransform.position);

		// Update the Player_Distance parameter in the Animator
		anim.SetFloat("Player_Distance", distance);

		// Enemy is dead
		if (Enemy.Health <= 0)
		{
			anim.SetBool("Death", true);
			
		}
		
	}
}
