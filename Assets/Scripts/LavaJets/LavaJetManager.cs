using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaJetManager : MonoBehaviour
{
	public GameObject lavaJetPrefab;
	public List<Transform> spawnPoints;
	public float lavaJetDuration = 4.0f; // Only defined here, not in LavaJet
	public float lavaJetInterval = 5.0f; // New variable for interval between lava jet spawns

	void Start()
	{
		StartCoroutine(SpawnLavaJets());
	}

	IEnumerator SpawnLavaJets()
	{
		foreach (Transform spawnPoint in spawnPoints)
		{
			GameObject lavaJet = Instantiate(lavaJetPrefab, spawnPoint.position, Quaternion.identity);
			lavaJet.GetComponent<LavaJet>().StartLavaJetCycle(lavaJetDuration, lavaJetInterval);

			// Optionally, destroy the object after the duration if you want one-time use
			//Destroy(lavaJet, lavaJetDuration);
		}
		
		// End the coroutine after spawning once
		yield break;
	}
}
