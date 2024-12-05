using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortcutManager : MonoBehaviour
{
	[HideInInspector]public Player player; // Reference to the Player object
	
	[Header("Damage Boost")]
	public KeyCode dmgBoostKey = KeyCode.Alpha7; // Key to activate damage boost
	public float damageMultiplier = 2f; // The multiplier value
	public float dmgDuration = 10f; // Buff duration in seconds
	
	[Header("Heal")]
	public KeyCode healKey = KeyCode.Alpha8; // Key to activate beam
	public int hpAmount = 50; // Amount of HP to recover
	
	[Header("Slow")]
	public KeyCode slowKey = KeyCode.Alpha9; // Key to activate enemy slow
	public float slowAmount = 0.5f; // Slow amount
	public float slowDuration = 5f; // Duration of slow effect
	
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
	
	// Update is called once per frame
	void Update()
	{
		if(Input.GetKeyDown(dmgBoostKey))
		{
			player.ApplyDamageBoost(damageMultiplier, dmgDuration);
		}
		else if(Input.GetKeyDown(healKey))
		{
			player.RecoverHealth(hpAmount);
		}
		else if(Input.GetKeyDown(slowKey))
		{
			// Find all enemies in the scene
			BasicEnemy[] enemies = FindObjectsOfType<BasicEnemy>();
			MiniBossController[] miniBosses = FindObjectsOfType<MiniBossController>();
			foreach (BasicEnemy enemy in enemies)
			{
				enemy.ApplySlow(slowAmount, slowDuration);
			}
			foreach (MiniBossController miniBoss in miniBosses)
			{
				miniBoss.ApplySlow(slowAmount, slowDuration);
			}
		}
	}
}
