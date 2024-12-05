using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 10f; // The detection range
    [SerializeField] private LayerMask enemyLayer; // To filter which objects are considered enemies
    
    private List<GameObject> enemiesInRange = new List<GameObject>();

    private void Update()
    {
        UpdateEnemiesInRange(); // Update the list of nearby enemies on every frame
    }

    // This method is used to get the list of enemies in range based on distance
    public List<GameObject> GetEnemiesInRange()
    {
        return enemiesInRange;
    }

    // This method will return the closest enemy from the list
   public GameObject GetClosestEnemy(GameObject excludeEnemy = null)
	{
		if (enemiesInRange.Count == 0)
			return null;

		GameObject closestEnemy = null;
		float closestDistance = Mathf.Infinity;

		Vector3 currentPosition = transform.position;

		foreach (var enemy in enemiesInRange)
		{
			if (enemy != null && enemy != excludeEnemy)  // Make sure to exclude the current enemy
			{
				float distance = Vector3.Distance(currentPosition, enemy.transform.position);
				if (distance < closestDistance)
				{
					closestDistance = distance;
					closestEnemy = enemy;
				}
			}
		}

		return closestEnemy;
	}

    // This method checks all enemies in range by using Physics.OverlapSphere
   private void UpdateEnemiesInRange()
	{
		enemiesInRange.RemoveAll(enemy => enemy == null);  // Remove destroyed enemies
		Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);

		foreach (var collider in colliders)
		{
			if (collider.CompareTag("Enemy"))
			{
				if (!enemiesInRange.Contains(collider.gameObject)) // Add only if not already in list
				{
					enemiesInRange.Add(collider.gameObject);
					Debug.Log($"Enemy detected: {collider.gameObject.name}");
				}
			}
		}
	}
}
