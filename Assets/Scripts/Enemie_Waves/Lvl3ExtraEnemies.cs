using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lvl3ExtraEnemies : MonoBehaviour
{
	public GameObject[] extraEnemies; // List of enemies to spawn
	public Transform[] spawnPoints;  // Array of spawn points

	private WaveManager waveManager;
	private bool extraEnemiesSpawned = false;

	// Start is called before the first frame update
	void Start()
	{
		waveManager = FindObjectOfType<WaveManager>();
	}

	// Update is called once per frame
	void Update()
	{
		if (waveManager.miniBossSpawned && !extraEnemiesSpawned)
		{
			SpawnExtraEnemies();
		}
	}

	void SpawnExtraEnemies()
	{
		// Iterate over enemies and spawn points
		for (int i = 0; i < extraEnemies.Length; i++)
		{
			// Ensure we don't exceed the number of spawn points
			if (i < spawnPoints.Length)
			{
				Instantiate(extraEnemies[i], spawnPoints[i].position, spawnPoints[i].rotation);
			}
			else
			{
				Debug.LogWarning("Not enough spawn points for all enemies!");
			}
		}
		extraEnemiesSpawned = true; 
	}
}
