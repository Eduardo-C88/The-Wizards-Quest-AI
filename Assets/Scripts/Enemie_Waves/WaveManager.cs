using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
	public GameObject goblinPrefab;
	public GameObject rangedEnemyPrefab; // Prefab do inimigo ranged
	public GameObject miniBossPrefab;
	public Transform miniBossSpawnPoint;
	public Transform[] spawnPoints;
	public Transform[] waypointsSet1, waypointsSet2, waypointsSet3;

	public float delayBetweenWaves = 5f;

	public int initialEnemies;
	public int waveCount = 5;
	public int currentWave = 0;
	private bool waveInProgress = false;
	public bool miniBossSpawned = false;

	public float rangedEnemyChance = 0.3f; // Chance de spawnar inimigo ranged (0.3 = 30%)
	public Text enemyCountText;
	private int currentEnemyCount = 0;
	
	public KeyCode finishWaveKey = KeyCode.K;
	public KeyCode finishAllWavesKey = KeyCode.L;
	
	// List to keep track of all spawned enemies
	private List<GameObject> activeEnemies = new List<GameObject>();

	void Update()
	{
		if (Input.GetKeyDown(finishWaveKey))
		{
			FinishCurrentWave();
		}

		if (Input.GetKeyDown(finishAllWavesKey))
		{
			FinishAllWaves();
		}
	}

	void Start()
	{
		StartCoroutine(StartWaves());
	}

	 IEnumerator StartWaves()
	{
		while (currentWave < waveCount)
		{
			waveInProgress = true;
			int enemiesToSpawn = initialEnemies + (currentWave * 6);
			yield return StartCoroutine(SpawnWave(enemiesToSpawn));
			waveInProgress = false;
		}

		if (!miniBossSpawned && currentWave >= waveCount)
		{
			SpawnMiniBoss();
			miniBossSpawned = true;
		}
	}

	IEnumerator SpawnWave(int enemyCount)
	{
		currentEnemyCount = enemyCount;
		UpdateEnemyCountText();
		
		// Delay spawn of enemies between waves
		yield return new WaitForSeconds(delayBetweenWaves);

		int enemiesPerSpawn = enemyCount / spawnPoints.Length;
		int remainingEnemies = enemyCount % spawnPoints.Length;

		int[] spawnCounts = new int[spawnPoints.Length];
		for (int i = 0; i < spawnPoints.Length; i++)
			spawnCounts[i] = enemiesPerSpawn;

		for (int i = 0; i < remainingEnemies; i++)
			spawnCounts[i]++;

		for (int i = 0; i < spawnPoints.Length; i++)
		{
			for (int j = 0; j < spawnCounts[i]; j++)
			{
				GameObject enemyPrefab = (rangedEnemyPrefab != null && Random.value < rangedEnemyChance) 
										 ? rangedEnemyPrefab 
										 : goblinPrefab;

				GameObject enemy = Instantiate(enemyPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
				enemyController enemyScript = enemy.GetComponent<enemyController>();

				if (i == 0) enemyScript.SetWaypoints(waypointsSet1);
				else if (i == 1) enemyScript.SetWaypoints(waypointsSet2);
				else if (i == 2) enemyScript.SetWaypoints(waypointsSet3);

				activeEnemies.Add(enemy); // Add enemy to the list of active enemies
				yield return new WaitForSeconds(1);
			}
		}

		yield return new WaitUntil(() => currentEnemyCount <= 0);
	}

	public void EnemyKilled(GameObject enemy)
	{
		currentEnemyCount--;
		UpdateEnemyCountText();

		// Remove the killed enemy from the activeEnemies list
		if (activeEnemies.Contains(enemy))
		{
			activeEnemies.Remove(enemy);
		}

		if (currentEnemyCount <= 0 && waveInProgress)
		{
			waveInProgress = false;
			currentWave++;

			if (currentWave < waveCount)
				StartCoroutine(StartWaves());
		}
	}

	void FinishCurrentWave()
	{
		if (waveInProgress)
		{
			DestroyAllActiveEnemies(); // Destroy all active enemies in the current wave
			currentEnemyCount = 0;
			UpdateEnemyCountText();
			waveInProgress = false;
			currentWave++;

			StopAllCoroutines(); // Stop ongoing spawn coroutine if any
			StartCoroutine(StartWaves()); // Start next wave if available
		}
	}

	void FinishAllWaves()
	{
		DestroyAllActiveEnemies(); // Destroy all active enemies in all waves
		StopAllCoroutines(); // Stop all ongoing coroutines
		currentWave = waveCount;
		currentEnemyCount = 0;
		UpdateEnemyCountText();
		
		waveInProgress = false;

		if (!miniBossSpawned)
		{
			SpawnMiniBoss();
			miniBossSpawned = true;
		}
	}

	void DestroyAllActiveEnemies()
	{
		foreach (GameObject enemy in activeEnemies)
		{
			if (enemy != null) Destroy(enemy); // Destroy each enemy GameObject
		}

		activeEnemies.Clear(); // Clear the list after destroying all enemies
	}

	void SpawnMiniBoss()
	{
		Debug.Log("Spawning Mini Boss!");
		Instantiate(miniBossPrefab, miniBossSpawnPoint.position, miniBossSpawnPoint.rotation);
	}

	private void UpdateEnemyCountText()
	{
		if (enemyCountText != null)
		{
			enemyCountText.text = "Enemies Left: " + currentEnemyCount;
		}
	}
}
