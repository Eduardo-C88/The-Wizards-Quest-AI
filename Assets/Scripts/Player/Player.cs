using System.Collections;
using UnityEngine;
using TMPro;  // Required for UI components
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
	[Header("Stats")]
	public int health;
	public int maxHealth;  // To display health ratio or for clamping purposes
	public float damageMultiplier = 1f; // Default damage multiplier
	private bool isDamageBoostActive = false; // Track if the damage boost is active

	[Header("UI Elements")]
	public TMP_Text healthText;  // Reference to the UI Text component
	
	[Header("Skills active")]
	public bool isSpellActive = false;

	void Start()
	{
		UpdateHealthUI();
	}

	public void TakeDamage(int damage)
	{
		health -= damage;

		if (health <= 0)
		{
			health = 0;  // Clamp health to 0
			Debug.Log("Player died");
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			// Recarregar a cena atual
        	SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
			//Destroy(gameObject);
		}

		UpdateHealthUI();
	}

	void UpdateHealthUI()
	{
		healthText.text = "Health: " + health.ToString() + "/" + maxHealth.ToString();  // Updates the UI Text
	}
	
	public void RecoverHealth(int amount)
	{
		health += amount;
		health = Mathf.Clamp(health, 0, maxHealth);  // Clamp health to maxHealth
		UpdateHealthUI();
	}

	// Coroutine to reset the damage boost after the duration expires
	private IEnumerator BoostDuration(float duration)
	{
		yield return new WaitForSeconds(duration);

		damageMultiplier = 1f; // Reset to default
		isDamageBoostActive = false;

		Debug.Log("Damage boost expired!");
	}

	public void ApplyDamageBoost(float multiplier, float duration)
	{
		if (isDamageBoostActive)
		{
			StopCoroutine("BoostDuration"); // Stop the ongoing buff timer
		}

		damageMultiplier = multiplier; // Apply the new damage multiplier
		isDamageBoostActive = true;

		Debug.Log("Damage boost activated for " + duration + " segundos!");

		// Start or restart the timer to remove the buff after the new duration
		StartCoroutine(BoostDuration(duration));
	}
	
	public void ApplyKnockback(Vector3 direction, float force)
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		if (rb != null)
		{
			FpPlayerMovement playerMovement = GetComponent<FpPlayerMovement>();
			if (playerMovement != null)
			{
				float knockbackMultiplier = playerMovement.isGrounded ? 1f : 0.1f;
				Vector3 knockbackForce = direction.normalized * force * knockbackMultiplier;
				rb.AddForce(knockbackForce, ForceMode.Impulse);
			}
		}
	}
}



