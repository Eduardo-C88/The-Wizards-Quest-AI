using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class Lvl4Special : MonoBehaviour
{
	[SerializeField] private LayerMask playerLayer;

	[Header("Stats")]
	public int damage = 200;
	public float radius = 150;
	public float speedBoostMultiplier = 0f;
	public float attackDuration = 5.0f;
	public float chargeTime = 2.0f;

	[Header("Cooldown")]
	public float cooldownDuration = 10.0f;

	[SerializeField] private bool isReady = false;  // Indicates if the spell is ready to be cast
	[SerializeField] private bool isActive = false; // To track if the sphere is active
	[SerializeField] private bool inRage = false;
	private MiniBossController miniBossController;
	
	private TextMeshProUGUI warningText; // Reference for TextMeshPro. Use `Text` for basic text.

	//[Header("Animation")]
	//public bool animTrack = false;
	//public Animator anim;

	void Start()
	{
		miniBossController = GetComponent<MiniBossController>();
		
		// Find the warning text dynamically in the scene
		warningText = GameObject.Find("SkillWarning").GetComponent<TextMeshProUGUI>();

		if (warningText != null)
			warningText.text = ""; // Clear the text initially
		else
			Debug.LogWarning("WarningText UI element not found in the scene!");
	}

	void Update()
	{
		if (miniBossController.health <= 350 && !inRage)
		{
			inRage = true;
			isReady = true;
		}

		if (isReady && !isActive && inRage)
		{
			StartCoroutine(ChargeAndActivateSkill());
		}
	}

	private IEnumerator ChargeAndActivateSkill()
	{
		miniBossController.skillActive = true;
		isReady = false; // Prevent multiple activations
		//animTrack = true; // Optionally trigger charging animation

		miniBossController.PauseMovement(true); // Stop movement

		// Display charging warning
		StartCoroutine(UpdateText("GET OFF THE GROUND\nCharging...", chargeTime));

		// Charge Time
		yield return new WaitForSeconds(chargeTime);

		// Activate the skill
		isActive = true;
		StartCoroutine(UpdateText("Attack Active!", attackDuration));
		//animTrack = false; // Optionally disable charging animation
		//anim.SetBool("Spin", true); // Example animation trigger
		
		StartCoroutine(PerformSkill());

		// Wait for attack duration
		yield return new WaitForSeconds(attackDuration);

		miniBossController.PauseMovement(false); // Resume movement

		// Deactivate the skill
		isActive = false;
		miniBossController.skillActive = false;
		//anim.SetBool("Spin", false); // Stop the animation

		// Start cooldown
		StartCoroutine(Cooldown());
	}

	private IEnumerator PerformSkill()
	{
		while (isActive)
		{
			DealDamage();
			yield return new WaitForSeconds(0.1f); // Damage ticks
		}
	}

	private void DealDamage()
	{
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, playerLayer);
		foreach (var c in hitColliders)
		{
			Player player = c.GetComponent<Player>();
			FpPlayerMovement playerMovement = c.GetComponent<FpPlayerMovement>();
			if (player != null && playerMovement.isGrounded && player.damageMultiplier != 0)
			{
				player.TakeDamage(damage);
			}
		}
	}

	private IEnumerator Cooldown()
	{
		isReady = false;
		yield return new WaitForSeconds(cooldownDuration);
		isReady = true;
	}

	private void OnDrawGizmos()
	{
		if (isActive) // Only draw the gizmo when expanding
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, radius);
		}
	}
	
	private IEnumerator UpdateText(string message, float duration)
	{
		if (warningText != null)
		{
			float timer = duration;
			while (timer > 0)
			{
				warningText.text = $"{message}\nTime Left: {timer:F1}s";
				timer -= Time.deltaTime;
				yield return null;
			}
			warningText.text = ""; // Clear the text after the duration
		}
	}
}
