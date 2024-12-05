using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
	public int beamDamage = 17;   // Base damage dealt by the beam
	public float damageInterval = 0.5f; // Time between damage ticks

	private List<GameObject> enemiesInBeam = new List<GameObject>(); // List of enemies in the beam
	private Coroutine damageCoroutine; // Reference to the damage coroutine

	private Player player; // Reference to the Player to access the damage multiplier

	void Start()
	{
		if (player == null)
		{
			player = FindObjectOfType<Player>();
			if (player == null)
			{
				Debug.LogError("Player not found! Make sure there is a Player object in the scene.");
			}
			else
			{
				Debug.Log("Player found!");
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Enemy"))
		{
			if (!enemiesInBeam.Contains(other.gameObject))
			{
				enemiesInBeam.Add(other.gameObject);
				Debug.Log("Enemy entered beam: " + other.name);
				if (damageCoroutine == null) // Start damage coroutine if not already running
				{
					damageCoroutine = StartCoroutine(DamageEnemiesOverTime());
				}
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Enemy"))
		{
			if (enemiesInBeam.Contains(other.gameObject))
			{
				enemiesInBeam.Remove(other.gameObject);
				Debug.Log("Enemy exited beam: " + other.name);
			}
		}
	}

	private IEnumerator DamageEnemiesOverTime()
	{
		Debug.Log("Started damaging enemies over time.");

		while (enemiesInBeam.Count > 0)
		{
			// Calculate the total damage using the player's damage multiplier
			int modifiedDamage = Mathf.RoundToInt(beamDamage * player.damageMultiplier);

			foreach (GameObject enemy in enemiesInBeam)
			{
				if (enemy != null && enemy.activeInHierarchy)
				{
					// Check if the enemy has a BasicEnemy component
					BasicEnemy basicEnemy = enemy.GetComponent<BasicEnemy>();
					if (basicEnemy != null)
					{
						basicEnemy.TakeDamage(modifiedDamage);
						Debug.Log("Dealt " + modifiedDamage + " damage to " + enemy.name);
					}

					// Check if the enemy has a MiniBossController component
					MiniBossController miniBoss = enemy.GetComponent<MiniBossController>();
					if (miniBoss != null)
					{
						miniBoss.TakeDamage(modifiedDamage);
						Debug.Log("Dealt " + modifiedDamage + " damage to mini-boss: " + enemy.name);
					}
					
					BossController boss = enemy.GetComponent<BossController>();
					if (boss != null)
					{
						boss.TakeDamage(modifiedDamage);
						Debug.Log("Dealt " + modifiedDamage + " damage to boss: " + enemy.name);
					}
				}
			}
			yield return new WaitForSeconds(damageInterval);
		}

		damageCoroutine = null; // Reset the coroutine when no enemies remain
		Debug.Log("Stopped damaging enemies.");
	}
}
