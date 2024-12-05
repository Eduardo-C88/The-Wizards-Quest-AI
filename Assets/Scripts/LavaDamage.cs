using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaDamage : MonoBehaviour
{
	public int damage = 10;
	public float damageInterval = 1f;
	public float damageStartDelay = 0.5f; // Delay before starting to deal damage
	public float slowFactor = 0.8f; // The slow factor to apply
	
	public bool isOnLava;

	private Coroutine damageCoroutine;
	private FpPlayerMovement player;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			isOnLava = true;
			
			player = other.GetComponent<FpPlayerMovement>();
			player.moveSpeed = player.moveSpeed * slowFactor;	

			// Start damage coroutine
			damageCoroutine = StartCoroutine(DealDamage(other.GetComponent<Player>()));
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player") && damageCoroutine != null)
		{
			isOnLava = false;
			player.moveSpeed = player.defaultMoveSpeed;
			
			StopCoroutine(damageCoroutine);
			damageCoroutine = null;
		}
	}

	private IEnumerator DealDamage(Player player)
	{
		yield return new WaitForSeconds(damageStartDelay); // Delay before starting to deal damage
		while (true)
		{
			player.TakeDamage(damage);
			yield return new WaitForSeconds(damageInterval);
		}
	}
}
