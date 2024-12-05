using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDamage : MonoBehaviour
{
	public int damage = 10;
	public float damageInterval = 1f;
	public float damageStartDelay = 0.5f; // Delay before starting to deal damage
	public float upwardForce = 25f; // The upward force to apply
	public float forceDelay = 2f; // Delay before applying upward force

	private Coroutine damageCoroutine;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			// Start the coroutine for upward force
			StartCoroutine(ApplyUpwardForce(other.GetComponent<Rigidbody>()));

			// Start damage coroutine
			damageCoroutine = StartCoroutine(DealDamage(other.GetComponent<Player>()));
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player") && damageCoroutine != null)
		{
			StopCoroutine(damageCoroutine);
			damageCoroutine = null;
			StopCoroutine(ApplyUpwardForce(other.GetComponent<Rigidbody>()));
		}
	}

	private IEnumerator ApplyUpwardForce(Rigidbody playerRigidbody)
	{
		yield return new WaitForSeconds(forceDelay); // Wait for the specified delay
		if (playerRigidbody != null)
		{
			playerRigidbody.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);
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
