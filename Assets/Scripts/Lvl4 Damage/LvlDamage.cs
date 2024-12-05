using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LvlDamage : MonoBehaviour
{
private Player player;
	public int damage = 2;
	public float damageInterval = 2.0f;
	
	Coroutine damageCoroutine;

	// Start is called before the first frame update
	void Start()
	{
		player = FindObjectOfType<Player>();
		if (player == null)
		{
			Debug.LogError("Player not found! Make sure there is a Player object in the scene.");
		}
		
		UpdateDamageStatus();
	}

	// Called when the player enters or exits the safe zone
	public void UpdateDamageStatus()
	{
		if (player == null) return;

		if (damageCoroutine == null)
		{
			// Start damaging the player if not in safe zone
			damageCoroutine = StartCoroutine(DealDamage());
		}
		else if (damageCoroutine != null)
		{
			// Stop damaging the player if in safe zone
			StopCoroutine(damageCoroutine);
			damageCoroutine = null;
		}
	}

	private IEnumerator DealDamage()
	{
		yield return new WaitForSeconds(damageInterval);
		
		while (true)
		{
			if (player != null)
			{
				player.TakeDamage(damage);
				Debug.Log("Dealing damage to player.");
			}
			yield return new WaitForSeconds(damageInterval);
		}
	}
}
