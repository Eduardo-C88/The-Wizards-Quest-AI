using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
	[Header("Damage in Safe Zone")]
	public float safeZoneDamageMultiplier = 0f;
	
	private LvlDamage lvlDmg;

	void Start()
	{
		lvlDmg = FindObjectOfType<LvlDamage>();
		if (lvlDmg == null)
		{
			Debug.LogError("LvlDmg component not found! Make sure it's attached to an object in the scene.");
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			if (other.TryGetComponent(out Player player))
			{
				lvlDmg.UpdateDamageStatus();  // Update damage status when entering safe zone
				player.damageMultiplier = safeZoneDamageMultiplier;
				Debug.Log("Player entered the safe zone.");
			}
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			if (other.TryGetComponent(out Player player))
			{
				lvlDmg.UpdateDamageStatus();  // Update damage status when exiting safe zone
				player.damageMultiplier = 1f;
				Debug.Log("Player exited the safe zone.");
			}
		}
	}
}
