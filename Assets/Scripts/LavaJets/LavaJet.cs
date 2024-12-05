using System.Collections;
using UnityEngine;

public class LavaJet : MonoBehaviour
{
	public int damage = 10;
	public float damageInterval = 1.0f;
	private Collider lavaCollider;
	private ParticleSystem lavaParticles;
	private Coroutine damageCoroutine;

	void Awake()
	{
		// Cache references to components
		lavaCollider = GetComponent<Collider>();
		lavaParticles = GetComponentInChildren<ParticleSystem>();

		// Initially deactivate collider and particles
		lavaCollider.enabled = false;
		if (lavaParticles != null)
		{
			lavaParticles.Stop();
		}
	}

	void OnTriggerEnter(Collider other)
	{
		Debug.Log("Lava jet trigger entered.");
		if (other.CompareTag("Player"))
		{
			Debug.Log("Player entered lava jet area.");
			Player player = other.GetComponent<Player>();

			if (player != null && damageCoroutine == null) // Ensure player component exists and coroutine isn't already running
			{
				damageCoroutine = StartCoroutine(DealDamage(player));
			}
			else if (player == null)
			{
				Debug.LogWarning("Player component not found on the object with 'Player' tag.");
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player") && damageCoroutine != null)
		{
			Debug.Log("Player exited lava jet area.");
			StopCoroutine(damageCoroutine);
			damageCoroutine = null;
		}
	}

	public void StartLavaJetCycle(float activeDuration, float inactiveDuration)
	{
		StartCoroutine(LavaJetCycle(activeDuration, inactiveDuration));
	}

	private IEnumerator LavaJetCycle(float activeDuration, float inactiveDuration)
	{
		while (true)
		{
			Activate();
			yield return new WaitForSeconds(activeDuration);
			Deactivate();
			yield return new WaitForSeconds(inactiveDuration);
		}
	}

	private void Activate()
	{
		Debug.Log("Lava jet activated.");
		lavaCollider.enabled = true;
		if (lavaParticles != null)
		{
			lavaParticles.Play();
		}
	}

	private void Deactivate()
	{
		Debug.Log("Lava jet deactivated.");
		lavaCollider.enabled = false;
		if (lavaParticles != null)
		{
			lavaParticles.Stop();
		}

		if (damageCoroutine != null)
		{
			StopCoroutine(damageCoroutine);
			damageCoroutine = null;
		}
	}

	private IEnumerator DealDamage(Player player)
	{
		while (true)
		{
			Debug.Log("Dealing damage to player.");
			player.TakeDamage(damage);
			yield return new WaitForSeconds(damageInterval);
		}
	}
}
