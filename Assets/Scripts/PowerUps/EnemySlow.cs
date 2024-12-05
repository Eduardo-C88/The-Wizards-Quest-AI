using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlow : MonoBehaviour
{
	public float slowAmount = 0.5f;
	public float duration = 5f;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.GetComponent<Player>() != null)
		{
 			// Find all enemies in the scene
			BasicEnemy[] enemies = FindObjectsOfType<BasicEnemy>();
			MiniBossController[] miniBosses = FindObjectsOfType<MiniBossController>();
			foreach (BasicEnemy enemy in enemies)
			{
				enemy.ApplySlow(slowAmount, duration);
			}
			foreach (MiniBossController miniBoss in miniBosses)
			{
				miniBoss.ApplySlow(slowAmount, duration);
			}
			Destroy(gameObject); // Destroy power-up after use
		}
	}
}
