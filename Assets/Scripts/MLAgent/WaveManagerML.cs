//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//public class WaveManagerML : MonoBehaviour
//{
//    public GameObject enemyPrefab;
//    public Transform[] spawnPoints;

//    private Dictionary<Transform, GameObject> spawnedEnemies = new Dictionary<Transform, GameObject>();

//    private void Start()
//    {
//        foreach (Transform spawnPoint in spawnPoints)
//        {
//            SpawnEnemy(spawnPoint);
//        }
//    }

//    private void SpawnEnemy(Transform spawnPoint)
//    {
//        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
//        spawnedEnemies[spawnPoint] = enemy;

//        BasicEnemyAgent agent = enemy.GetComponent<BasicEnemyAgent>();
//        if (agent != null)
//        {
//            agent.player = FindObjectOfType<PlayerStats>()?.transform;
//        }
//    }

//    public void EnemyDefeated(GameObject enemy)
//    {
//        if (spawnedEnemies.ContainsValue(enemy))
//        {
//            Transform spawnPoint = spawnedEnemies.FirstOrDefault(x => x.Value == enemy).Key;
//            spawnedEnemies[spawnPoint] = null;

//            StartCoroutine(RespawnEnemy(spawnPoint));
//        }
//    }

//    public IEnumerator RespawnEnemy(Transform spawnPoint)
//    {
//        yield return new WaitForSeconds(2f); // Adjust respawn delay as needed
//        if (spawnPoint != null && spawnedEnemies[spawnPoint] == null)
//        {
//            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
//            spawnedEnemies[spawnPoint] = newEnemy;

//            BasicEnemyAgent agent = newEnemy.GetComponent<BasicEnemyAgent>();
//            if (agent != null)
//            {
//                agent.player = FindObjectOfType<PlayerStats>()?.transform;
//            }
//        }
//    }

//    public void ResetWaves()
//    {
//        foreach (var enemy in spawnedEnemies.Values)
//        {
//            if (enemy != null)
//            {
//                Destroy(enemy);
//            }
//        }

//        spawnedEnemies.Clear();

//        foreach (Transform spawnPoint in spawnPoints)
//        {
//            SpawnEnemy(spawnPoint);
//        }

//        Debug.Log("WaveManager reset: All enemies respawned.");
//    }
//}